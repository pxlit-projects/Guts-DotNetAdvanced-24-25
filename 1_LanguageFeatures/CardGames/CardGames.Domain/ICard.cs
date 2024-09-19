namespace CardGames.Domain;

public interface ICard
{
    public CardSuit Suit { get; }
    public CardRank Rank { get; }
}