using System.Reflection;
using Guts.Client.Core;

namespace CardGames.Domain.Tests;

[ExerciseTestFixture("dotNet2", "1-LanguageFeatures", "CardGames",
    @"CardGames.Domain\Card.cs;CardGames.Domain\ICard.cs")]
public class CardTests
{
    [MonitoredTest("Card - Should implement ICard")]
    public void _01_ShouldImplementICard()
    {
        TestHelper.AssertFileHasNotChanged(@"CardGames.Domain\ICard.cs", "6B-53-9E-0C-EB-6B-2A-FB-64-1A-9D-3A-AC-5E-B3-DE");
        Assert.That(typeof(Card).IsAssignableTo(typeof(ICard)));
    }

    [MonitoredTest("Card - Should be a value type")]
    public void _02_ShouldBeAValueType()
    {
        Assert.That(typeof(Card).IsValueType);
    }

    [MonitoredTest("Card - Properties Suit and Rank should only be assignable in the constructor or at declaration")]
    public void _03_PropertiesSuitAndRankShouldOnlyBeAssignableInTheConstructorOrDeclaration()
    {
        PropertyInfo[] properties = typeof(Card).GetProperties();

        PropertyInfo? suitProperty = properties.FirstOrDefault(p => p.Name == nameof(ICard.Suit));
        Assert.That(suitProperty, Is.Not.Null, "The property 'Suit' should exist on the 'Card' type");
        Assert.That(suitProperty!.SetMethod, Is.Null, "The property 'Suit' is settable in other methods than the constructor.");

        PropertyInfo? rankProperty = properties.FirstOrDefault(p => p.Name == nameof(ICard.Rank));
        Assert.That(rankProperty, Is.Not.Null, "The property 'Rank' should exist on the 'Card' type");
        Assert.That(rankProperty!.SetMethod, Is.Null, "The property 'Rank' is settable in other methods than the constructor.");
    }

    [MonitoredTest("Card - ToString - Should return text with rank and suit")]
    [TestCase(CardRank.Ace, CardSuit.Spades, "Ace of Spades")]
    [TestCase(CardRank.Jack, CardSuit.Hearts, "Jack of Hearts")]
    [TestCase(CardRank.Seven, CardSuit.Clubs, "Seven of Clubs")]
    [TestCase(CardRank.Ten, CardSuit.Diamonds, "Ten of Diamonds")]
    public void _04_ToString_ShouldReturnTextWithRankAndSuit(CardRank rank, CardSuit suit, string expected)
    {
        Assert.That(new Card(suit, rank).ToString(), Is.EqualTo(expected));
    }
}