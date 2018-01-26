using PokerHands.Data.Enums;

namespace PokerHands.Data
{
    public class PlayingCard
    {
        public Suit Suit { get; set; }
        public CardValue CardValue { get; set; }
        public PlayingCard(Suit suit, CardValue cardValue)
        {
            Suit = suit;
            CardValue = cardValue;
        }
    }
}