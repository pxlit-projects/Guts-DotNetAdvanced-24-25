using System.Reflection;
using Castle.Core.Internal;
using Guts.Client.Core;
using Moq;
using NUnit.Framework.Internal;

namespace CardGames.Domain.Tests;

[ExerciseTestFixture("dotNet2", "1-LanguageFeatures", "CardGames",
    @"CardGames.Domain\HigherLowerGame.cs")]
public class HigherLowerGameTests
{
    private static readonly Random Random = new Random();

    private Mock<ICardDeck> _standardDeckMock = null!;
    private Mock<ICardDeck> _filteredDeckMock = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _standardDeckMock = new Mock<ICardDeck>();
        _filteredDeckMock = new Mock<ICardDeck>();
        _standardDeckMock.Setup(deck => deck.WithoutCardsRankingLowerThan(It.IsAny<CardRank>()))
            .Returns(_filteredDeckMock.Object);
    }

    [MonitoredTest("HigherLowerGame - Constructor - Should remove the cards below the minimum and shuffle the deck")]
    public void _01_Constructor_ShouldRemoveBelowMinimumCardsAndShuffleTheDeck()
    {
        //Arrange
        CardRank minimumRank = (CardRank)Random.Next(3, 10);

        //Act
        var game = new HigherLowerGame(_standardDeckMock.Object, 1, minimumRank);

        //Assert
        _standardDeckMock.Verify(deck => deck.WithoutCardsRankingLowerThan(minimumRank), Times.Once,
            "The 'WithoutCardsRankingLowerThan' method is not called correctly on the passed in standard deck.");
        _filteredDeckMock.Verify(deck => deck.Shuffle(), Times.Once,
            "The 'Shuffle' method is not called on the deck that does not contain cards below the minimum rank.");
    }

    [MonitoredTest("HigherLowerGame - Constructor - Should deal a first card")]
    public void _02_Constructor_ShouldDealAFirstCard()
    {
        //Arrange
        var firstCardMock = new Mock<ICard>();
        _filteredDeckMock.Setup(deck => deck.DealCard()).Returns(firstCardMock.Object);

        //Act
        var game = new HigherLowerGame(_standardDeckMock.Object, 1);

        //Assert
        Assert.That(game.CurrentCard, Is.SameAs(firstCardMock.Object),
            "The 'CurrentCard' should be a card dealt from the deck that does not contain cards below the minimum rank.");
        Assert.That(game.PreviousCard, Is.Null, "There should not be a previous card after construction.");
    }

    [MonitoredTest("HigherLowerGame - MakeGuess - First correct guess of 5 - Should increase the number of correct guesses while the motivation stays null")]
    [TestCase(CardRank.Five, CardRank.King, true)]
    [TestCase(CardRank.Seven, CardRank.Seven, true)]
    [TestCase(CardRank.Jack, CardRank.Ten, false)]
    public void _03_MakeGuess_FirstCorrectGuessOf5_ShouldIncreaseNumberOfCorrectGuesses_MotivationStaysNull(CardRank rankOfFirstCard, CardRank rankOfSecondCard, bool guessHigher)
    {
        //Arrange
        var firstCardMock = CardMockBuilder.Build(rankOfFirstCard, CardSuit.Clubs);
        var secondCardMock = CardMockBuilder.Build(rankOfSecondCard, CardSuit.Hearts);
        _filteredDeckMock.SetupSequence(deck => deck.DealCard())
            .Returns(firstCardMock.Object)
            .Returns(secondCardMock.Object);

        var game = new HigherLowerGame(_standardDeckMock.Object, 5);

        //Act
        game.MakeGuess(higher: guessHigher);

        //Assert
        Assert.That(game.CurrentCard, Is.SameAs(secondCardMock.Object), "The second dealt card should now be the CurrentCard");
        Assert.That(game.PreviousCard, Is.SameAs(firstCardMock.Object), "The first dealt card should now be the FirstCard");
        Assert.That(game.NumberOfCorrectGuesses, Is.EqualTo(1), "The NumberOfCorrectGuesses should increase");
        Assert.That(game.Motivation, Is.Null,
            "There should not be a Motivation if their are more that 3 correct guesses needed.");
    }

    [MonitoredTest("HigherLowerGame - MakeGuess - 4 correct guesses in a row - Should set a motivation when 3 or less correct guesses are needed")]
    public void _04_MakeGuess_4CorrectGuessesInARow_ShouldSetAMotivationWhen3OrLessCorrectGuessesAreNeeded()
    {
        //Arrange
        var ranks = new[] { CardRank.Five, CardRank.King, CardRank.Seven, CardRank.Seven, CardRank.Jack };
        _filteredDeckMock.SetupSequence(deck => deck.DealCard())
            .Returns(CardMockBuilder.Build(ranks[0], CardSuit.Clubs).Object)
            .Returns(CardMockBuilder.Build(ranks[1], CardSuit.Diamonds).Object)
            .Returns(CardMockBuilder.Build(ranks[2], CardSuit.Hearts).Object)
            .Returns(CardMockBuilder.Build(ranks[3], CardSuit.Spades).Object)
            .Returns(CardMockBuilder.Build(ranks[4], CardSuit.Clubs).Object);

        var game = new HigherLowerGame(_standardDeckMock.Object, 5);

        //Act + Assert
        game.MakeGuess(higher: true);
        Assert.That(game.NumberOfCorrectGuesses, Is.EqualTo(1), "The NumberOfCorrectGuesses should increase");
        Assert.That(game.Motivation, Is.Null,
            "There should not be a Motivation if their are more that 3 correct guesses needed.");
        Assert.That(game.HasWon, Is.False, "HasWon should be false.");

        game.MakeGuess(higher: false);
        Assert.That(game.NumberOfCorrectGuesses, Is.EqualTo(2), "The NumberOfCorrectGuesses should increase");
        Assert.That(game.Motivation, Does.Contain("3"),
            "The Motivation should be set to say that only 3 more correct guesses are needed.");
        Assert.That(game.HasWon, Is.False, "HasWon should be false.");

        game.MakeGuess(higher: false);
        Assert.That(game.NumberOfCorrectGuesses, Is.EqualTo(3), "The NumberOfCorrectGuesses should increase");
        Assert.That(game.Motivation, Does.Contain("2"),
            "The Motivation should be set to say that only 2 more correct guesses are needed.");
        Assert.That(game.HasWon, Is.False, "HasWon should be false.");

        game.MakeGuess(higher: true);
        Assert.That(game.NumberOfCorrectGuesses, Is.EqualTo(4), "The NumberOfCorrectGuesses should increase");
        Assert.That(game.Motivation, Does.Contain("1"),
            "The Motivation should be set to say that only 1 more correct guess is needed.");
        Assert.That(game.HasWon, Is.False, "HasWon should be false.");
    }

    [MonitoredTest("HigherLowerGame - MakeGuess - Required number of correct guesses in a row - HasWon should return true")]
    public void _05_MakeGuess_RequiredNumberOfCorrectGuessesInARow_HasWonShouldReturnTrue()
    {
        //Arrange
        var ranks = new[] { CardRank.Five, CardRank.Ace, CardRank.Seven};
        _filteredDeckMock.SetupSequence(deck => deck.DealCard())
            .Returns(CardMockBuilder.Build(ranks[0], CardSuit.Clubs).Object)
            .Returns(CardMockBuilder.Build(ranks[1], CardSuit.Diamonds).Object)
            .Returns(CardMockBuilder.Build(ranks[2], CardSuit.Hearts).Object);

        var game = new HigherLowerGame(_standardDeckMock.Object, 2);

        //Act + Assert
        game.MakeGuess(higher: false);
        game.MakeGuess(higher: true);
        Assert.That(game.HasWon, Is.True, "HasWon should be true after 2 correct guesses.");
    }

    [MonitoredTest("HigherLowerGame - MakeGuess - Wrong guess - Should reset number of correct guesses and motivation")]
    public void _06_MakeGuess_WrongGuess_ShouldResetNumberOfCorrectGuessesAndMotivation()
    {
        //Arrange
        var ranks = new[] { CardRank.Five, CardRank.Ace, CardRank.Seven };
        _filteredDeckMock.SetupSequence(deck => deck.DealCard())
            .Returns(CardMockBuilder.Build(ranks[0], CardSuit.Clubs).Object)
            .Returns(CardMockBuilder.Build(ranks[1], CardSuit.Diamonds).Object)
            .Returns(CardMockBuilder.Build(ranks[2], CardSuit.Hearts).Object);

        var game = new HigherLowerGame(_standardDeckMock.Object, 2);

        //Act + Assert
        game.MakeGuess(higher: false);
        game.MakeGuess(higher: false);
        Assert.That(game.NumberOfCorrectGuesses, Is.Zero, "NumberOfCorrectGuesses should be zero after wrong guess.");
        Assert.That(game.Motivation, Is.Null, "Motivation should be null after wrong guess.");
        Assert.That(game.HasWon, Is.False, "HasWon should be false when second guess is wrong.");
    }

    [MonitoredTest("HigherLowerGame - Properties should be publicly readable and only writable in the class itself")]
    public void _07_Properties_ShouldBePubliclyReadableAndOnlyWritableInTheClassItself()
    {
        AssertIsPublicReadButNotWriteProperty(nameof(HigherLowerGame.CurrentCard));
        AssertIsPublicReadButNotWriteProperty(nameof(HigherLowerGame.PreviousCard));
        AssertIsPublicReadButNotWriteProperty(nameof(HigherLowerGame.NumberOfCorrectGuesses));
        AssertIsPublicReadButNotWriteProperty(nameof(HigherLowerGame.Motivation));
        AssertIsPublicReadButNotWriteProperty(nameof(HigherLowerGame.HasWon), shouldHaveSetter: false);
    }

    [MonitoredTest("HigherLowerGame - Should use Nullable Reference Types for some properties")]
    public void _08_ShouldUseNullableReferenceTypesForSomeProperties()
    {
        AssertNullableReferenceType(nameof(HigherLowerGame.CurrentCard), false);
        AssertNullableReferenceType(nameof(HigherLowerGame.PreviousCard), true);
        AssertNullableReferenceType(nameof(HigherLowerGame.Motivation), true);
    }

    private void AssertIsPublicReadButNotWriteProperty(string propertyName, bool shouldHaveSetter = true)
    {
        PropertyInfo? property = typeof(HigherLowerGame).GetProperty(propertyName);
        Assert.That(property, Is.Not.Null, $"The '{propertyName}' property should be defined.");
        Assert.That(property!.GetMethod, Is.Not.Null, $"The '{propertyName}' property should have a get method.");
        Assert.That(property.GetMethod!.IsPublic, Is.True,
            $"The '{propertyName}' property is not publicly readable.");

        if (shouldHaveSetter)
        {
            Assert.That(property!.SetMethod, Is.Not.Null, $"The '{propertyName}' property should have a set method.");
            Assert.That(property.SetMethod!.IsPublic, Is.False,
                $"The '{propertyName}' property can be changed from outside the class.");
        }
        else
        {
            Assert.That(property!.SetMethod, Is.Null, $"The '{propertyName}' property should not have a set method.");
        }
    }

    private void AssertNullableReferenceType(string propertyName, bool shouldBeNullable)
    {
        PropertyInfo? property = typeof(HigherLowerGame).GetProperty(propertyName);
        Assert.That(property, Is.Not.Null, $"The '{propertyName}' property should be defined.");

        NullabilityInfo nullabilityInfo = new NullabilityInfoContext().Create(property!);

        bool isNullableReferenceType = nullabilityInfo.WriteState == NullabilityState.Nullable;

        if (shouldBeNullable)
        {
            Assert.That(isNullableReferenceType, Is.True,
                               $"The '{propertyName}' property should be a nullable reference type.");
        }
        else
        {
            Assert.That(isNullableReferenceType, Is.False,
                               $"The '{propertyName}' property should not be a nullable reference type.");
        }
    }
}