namespace CardGames.Domain;

public class HigherLowerGame
{
    public ICard CurrentCard { get; set; }
    public ICard PreviousCard { get; set; }

    public int NumberOfCorrectGuesses { get; set; }

    public string Motivation { get; set; }

    public bool HasWon { get; set; }

    public HigherLowerGame(ICardDeck standardDeck, int requiredNumberOfCorrectGuesses, CardRank minimumRank = CardRank.Ace)
    {
        throw new NotImplementedException("HigherLowerGame constructor not implemented yet.");
    }

    public void MakeGuess(bool higher)
    {
        throw new NotImplementedException("MakeGuess method not implemented yet.");
    }
}