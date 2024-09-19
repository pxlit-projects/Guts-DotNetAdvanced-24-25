using Guts.Client.Core;

namespace CardGames.Domain.Tests
{
    [ExerciseTestFixture("dotNet2", "1-LanguageFeatures", "CardGames",
        @"CardGames.Domain\CardRank.cs")]
    public class CardRankTests
    {
        [MonitoredTest("CardRank - CardRank value should match actual card value")]
        public void CardRankValueShouldMatchActualCardValue()
        {
            Assert.That((int)CardRank.Ace, Is.EqualTo(1), "The rank 'Ace' should be equivalent to 1");
            Assert.That((int)CardRank.Two, Is.EqualTo(2), "The rank 'Two' should be equivalent to 2");
            Assert.That((int)CardRank.Three, Is.EqualTo(3), "The rank 'Three' should be equivalent to 3");
            Assert.That((int)CardRank.Four, Is.EqualTo(4), "The rank 'Four' should be equivalent to 4");
            Assert.That((int)CardRank.Five, Is.EqualTo(5), "The rank 'Five' should be equivalent to 5");
            Assert.That((int)CardRank.Six, Is.EqualTo(6), "The rank 'Six' should be equivalent to 6");
            Assert.That((int)CardRank.Seven, Is.EqualTo(7), "The rank 'Seven' should be equivalent to 7");
            Assert.That((int)CardRank.Eight, Is.EqualTo(8), "The rank 'Eight' should be equivalent to 8");
            Assert.That((int)CardRank.Nine, Is.EqualTo(9), "The rank 'Nine' should be equivalent to 9");
            Assert.That((int)CardRank.Ten, Is.EqualTo(10), "The rank 'Ten' should be equivalent to 10");
            Assert.That((int)CardRank.Jack, Is.EqualTo(11), "The rank 'Jack' should be equivalent to 11");
            Assert.That((int)CardRank.Queen, Is.EqualTo(12), "The rank 'Queen' should be equivalent to 12");
            Assert.That((int)CardRank.King, Is.EqualTo(13), "The rank 'King' should be equivalent to 13");
        }
    }
}