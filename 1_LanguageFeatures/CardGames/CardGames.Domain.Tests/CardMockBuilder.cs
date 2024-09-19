using Moq;

namespace CardGames.Domain.Tests;

internal static class CardMockBuilder
{
    public static Mock<ICard> Build(CardRank rank, CardSuit suit)
    {
        var cardMock = new Mock<ICard>();
        cardMock.Setup(card => card.Rank).Returns(rank);
        cardMock.Setup(card => card.Suit).Returns(suit);
        return cardMock;
    }
}