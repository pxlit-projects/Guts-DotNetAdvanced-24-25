using System.Reflection;
using Guts.Client.Core.TestTools;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework.Interfaces;
using Guts.Client.Core;

namespace CardGames.Domain.Tests;

[ExerciseTestFixture("dotNet2", "1-LanguageFeatures", "CardGames", 
    @"CardGames.Domain\CardDeck.cs;CardGames.Domain\ICardDeck.cs")]
public class CardDeckTests
{
    private static readonly Random Random = new Random();
    private readonly string _code;

    public CardDeckTests()
    {
        _code = CodeCleaner.StripComments(Solution.Current.GetFileContent(@"CardGames.Domain\CardDeck.cs"));
    }

    [MonitoredTest("CardDeck - Should implement ICardDeck")]
    public void _01_ShouldImplementICardDeck()
    {
        Assert.That(typeof(CardDeck).IsAssignableTo(typeof(ICardDeck)), "CardDeck should implement ICardDeck");
    }

    [MonitoredTest("CardDeck - Parameter-less constructor should initialize a deck of 52 cards")]
    public void _02_ParameterlessConstructor_ShouldInitializeADeckOf52Cards()
    {
        _01_ShouldImplementICardDeck();
        IList<ICard> cards = GetCardsFieldValue(new CardDeck() as ICardDeck);

        Assert.That(cards.Count, Is.EqualTo(52), "The deck should contain 52 cards");
        foreach (CardSuit suit in Enum.GetValues<CardSuit>())
        {
            foreach (CardRank rank in Enum.GetValues<CardRank>())
            {
                Assert.That(cards.Any(card => card.Suit == suit && card.Rank == rank), $"{rank} of {suit} is missing in the deck.");
            }
        }
    }

    [MonitoredTest("CardDeck - Should use a private static field of type Random")]
    public void _03_ShouldUseAPrivateStaticFieldOfTypeRandom()
    {
        FieldInfo? randomField = typeof(CardDeck).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
            .SingleOrDefault(field => field.FieldType == typeof(Random));

        Assert.That(randomField, Is.Not.Null,
            "The class should have a private static field of type Random.");

        Assert.That(randomField!.IsInitOnly, Is.True,
            "Make sure the field that holds the Random instance can only be set in the constructor");

        Random? random = randomField.GetValue(new CardDeck()) as Random;

        Assert.That(random, Is.Not.Null, "Make sure the Random field is initialized so that it can be used (across instances)");
    }

    [MonitoredTest("CardDeck - Shuffle - Should randomly change the positions of the cards in the deck")]
    public void _04_Shuffle_ShouldRandomlyChangeThePositionsOfTheCardsInTheDeck()
    {
        _02_ParameterlessConstructor_ShouldInitializeADeckOf52Cards();

        ICardDeck deck = (new CardDeck() as ICardDeck)!;

        IList<ICard> cards = GetCardsFieldValue(deck);

        IList<ICard> shuffledCards = AssertShufflesRandomly(deck, cards);
        shuffledCards = AssertShufflesRandomly(deck, shuffledCards);  //Shuffle again to make sure it's not just a coincidence
        AssertShufflesRandomly(deck, shuffledCards);  //Shuffle again to make sure it's not just a coincidence
    }

    [MonitoredTest("CardDeck - DealCard - Cards present in the deck - Should remove the last card and return it")]
    public void _05_DealCard_CardsPresentInTheDeck_ShouldRemoveTheLastCardAndReturnIt()
    {
        _02_ParameterlessConstructor_ShouldInitializeADeckOf52Cards();

        ICardDeck deck = (new CardDeck() as ICardDeck)!;
        IList<ICard> cards = GetCardsFieldValue(deck);

        ICard card = deck.DealCard();
        Assert.That(card, Is.SameAs(cards[51]), "The card returned is not the last card of the deck");

        cards = GetCardsFieldValue(deck);
        Assert.That(cards.Count, Is.EqualTo(51), "The deck should contain 51 cards after dealing one card");

        Assert.That(cards.Any(c => c.Suit == card.Suit && c.Rank == card.Rank), Is.False,
                       "The card that was dealt should not be present in the deck anymore");
    }

    [MonitoredTest("CardDeck - DealCard - Try to deal 53 cards - Should throw InvalidOperationException")]
    public void _05_DealCard_TryToDeal53Cards_ShouldThrowInvalidOperationException()
    {
        _02_ParameterlessConstructor_ShouldInitializeADeckOf52Cards();

        ICardDeck deck = (new CardDeck() as ICardDeck)!;

        for (int i = 0; i < 52; i++)
        {
            deck.DealCard();
        }

        Assert.That(() => deck.DealCard(), Throws.TypeOf<InvalidOperationException>(),
                       "Trying to deal a card from an empty deck should throw an InvalidOperationException");
    }

    [MonitoredTest("CardDeck - Should have a private constructor that accepts a collection of cards")]
    public void _06_ShouldHaveAPrivateConstructorThatAcceptsACollectionOfCards()
    {
        _01_ShouldImplementICardDeck();
        ConstructorInfo? constructor = typeof(CardDeck).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(ctor =>
                ctor.GetParameters().Length == 1 &&
                ctor.GetParameters()[0].ParameterType.IsAssignableTo(typeof(IEnumerable<ICard>)));

        Assert.That(constructor, Is.Not.Null, "Cannot find a constructor that accepts an IEnumerable of ICard");
        Assert.That(constructor!.IsPrivate, Is.True, "The constructor that accepts an IEnumerable of ICard should be private");

        //Create a deck with only 4 cards
        ICard aceOfClubs = CardMockBuilder.Build(CardRank.Ace, CardSuit.Clubs).Object;
        ICard twoOfClubs = CardMockBuilder.Build(CardRank.Two, CardSuit.Clubs).Object;
        ICard threeOfClubs = CardMockBuilder.Build(CardRank.Three, CardSuit.Clubs).Object;
        ICard fourOfClubs = CardMockBuilder.Build(CardRank.Four, CardSuit.Clubs).Object;

        var cards = new List<ICard>
        {
            aceOfClubs, twoOfClubs, threeOfClubs, fourOfClubs
        };
        CardDeck deck = (constructor.Invoke(new object[]
        {
            cards
        }) as CardDeck)!;

        IList<ICard> cardsInDeck = GetCardsFieldValue(deck as ICardDeck);

        Assert.That(cardsInDeck, Is.EquivalentTo(cards),
            "The cards in the deck should be the same cards that were passed in as parameter");
    }

    [MonitoredTest("CardDeck - WithoutCardRankingLowerThan - Should return a new deck containing only the cards ranked equal or above the minimum rank")]
    public void _07_WithoutCardsRankingLowerThan_ShouldReturnANewDeckContainingOnlyTheCardsRankedEqualOrAboveTheMinimumRank()
    {
        try
        {
            var cardRankTests = new CardRankTests();
            cardRankTests.CardRankValueShouldMatchActualCardValue();
        }
        catch (Exception e)
        {
            Assert.Fail("Make sure you have implemented the CardRank enum correctly and that the CardRankTests pass.");
        }

        _02_ParameterlessConstructor_ShouldInitializeADeckOf52Cards();

        AssertDoesNotUseLoops(nameof(ICardDeck.WithoutCardsRankingLowerThan));

        ICardDeck deck = (new CardDeck() as ICardDeck)!;

        CardRank minimumRank = (CardRank)Random.Next(3, 10);

        ICardDeck filteredDeck = deck.WithoutCardsRankingLowerThan(minimumRank);
        IList<ICard> cards = GetCardsFieldValue(filteredDeck);

        Assert.That(cards.All(card => card.Rank >= minimumRank), "The resulting deck contains cards below the minimumRank");
        Assert.That(cards.Count, Is.EqualTo(52 - ((int)minimumRank - 1) * 4), "The resulting deck does not contain the right amount of cards");
    }

    [MonitoredTest("CardDeck - SplitBySuit - Should create 4 new decks - One deck for each suit")]
    public void _08_SplitBySuit_ShouldCreate4NewDecks_OneForEachSuit()
    {
        _02_ParameterlessConstructor_ShouldInitializeADeckOf52Cards();

        AssertDoesNotUseLoops(nameof(ICardDeck.SplitBySuit));

        ICardDeck deck = (new CardDeck() as ICardDeck)!;

        IList<CardDeck> splitDecks = deck.SplitBySuit();
        IList<IList<ICard>> cardsInSplitDecks = splitDecks.Select(d => GetCardsFieldValue(d as ICardDeck)).ToList();

        Assert.That(splitDecks.Count, Is.EqualTo(4), "The resulting list should contain 4 decks");

        foreach (CardSuit suit in Enum.GetValues<CardSuit>())
        {
            IList<ICard>? matchingDeckCards = cardsInSplitDecks.FirstOrDefault(cards => cards.FirstOrDefault()?.Suit == suit);
            Assert.That(matchingDeckCards, Is.Not.Null, $"Cannot find a deck that contains cards of {suit}");
            Assert.That(matchingDeckCards!.Count, Is.EqualTo(13),
                $"The deck that contains cards of {suit} should contain 13 cards");
            Assert.That(matchingDeckCards.All(card => card.Suit == suit),
                $"The deck that contains cards of {suit} should contain only cards of {suit}");
        }
    }

    [MonitoredTest("CardDeck - SplitBySuit - Should be marked as Obsolete in the interface definition")]
    public void _09_SplitBySuit_ShouldBeMarkedAsObsoleteInTheInterfaceDefinition()
    {
        _01_ShouldImplementICardDeck();

        MethodInfo? splitBySuitMethod = typeof(ICardDeck).GetMethods().SingleOrDefault(m => m.Name == nameof(ICardDeck.SplitBySuit));

        Assert.That(splitBySuitMethod, Is.Not.Null, $"Cannot find a method with name '{nameof(ICardDeck.SplitBySuit)}'");

        ObsoleteAttribute? obsoleteAttribute =
            splitBySuitMethod!.GetCustomAttributes(typeof(ObsoleteAttribute), false).FirstOrDefault() as
                ObsoleteAttribute;

        Assert.That(obsoleteAttribute, Is.Not.Null,
            $"The method '{nameof(ICardDeck.SplitBySuit)}' should be marked as obsolete in the definition of ICardDeck");

        Assert.That(obsoleteAttribute!.Message, Does.Contain("removed").IgnoreCase,
            "The obsolete message should contain the word 'removed'");
    }

    private void AssertDoesNotUseLoops(string methodName)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(_code);
        CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

        MethodDeclarationSyntax? methodDeclaration = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(ms => ms.Identifier.Text == methodName);

        Assert.That(methodDeclaration, Is.Not.Null, $"Cannot find a method with name '{methodName}'");

        BlockSyntax? methodBody = methodDeclaration!.Body;

        Assert.That(methodBody, Is.Not.Null, $"The method '{methodName}' does not have a body");

        var walker = new LoopWalker();
        walker.Visit(methodBody);

        Assert.That(walker.ContainsLoops, Is.False,
            $"The method '{methodName}' should not contain any loops (while, for). Use LINQ instead.");
    }

    private IList<ICard> AssertShufflesRandomly(ICardDeck deck, IList<ICard> originalCards)
    {
        deck.Shuffle();

        IList<ICard> shuffledCards = GetCardsFieldValue(deck);

        int numberOfDifferences = 0;
        for (var index = 0; index < originalCards.Count; index++)
        {
            ICard originalCard = originalCards[index];
            ICard shuffledCard = shuffledCards[index];

            if (originalCard.Suit != shuffledCard.Suit || originalCard.Rank != shuffledCard.Rank)
            {
                numberOfDifferences++;
            }
        }

        Assert.That(numberOfDifferences, Is.GreaterThan(30),
            "The cards should be shuffled better. " +
            "At least 30 cards should have another position each time the deck is shuffled.");

        return shuffledCards;
    }


    private IList<ICard> GetCardsFieldValue(ICardDeck deck)
    {
        _01_ShouldImplementICardDeck();
        FieldInfo? cardsField = typeof(CardDeck).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(field => field.FieldType.IsAssignableTo(typeof(IEnumerable<ICard>)));

        Assert.That(cardsField, Is.Not.Null,
            "The class should have a private field that holds a collection of ICard's. Cannot find such a field.");

        Assert.That(cardsField!.IsInitOnly, Is.True,
            "Make sure the field that holds the collection of cards can only be set in the constructor");

        IList<ICard> cards = new List<ICard>((cardsField.GetValue(deck) as IEnumerable<ICard>)!);
        return cards;
    }
}