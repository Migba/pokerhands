using System.Collections.Generic;
using PokerHands.Data;

namespace PokerHands.Solver
{
    public class PlayingCardComparer : IEqualityComparer<PlayingCard>
    {
        public bool Equals(PlayingCard x, PlayingCard y)
        {
            if (x == null || y == null) { return false; }
            return x.Suit == y.Suit && x.CardValue == y.CardValue;
        }

        public int GetHashCode(PlayingCard playingCard)
        {
            return playingCard.Suit.GetHashCode() + playingCard.CardValue.GetHashCode();
        }
    }
}