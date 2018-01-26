using System.Collections.Generic;
using PokerHands.Solver;

namespace PokerHands.Data
{
    public class PlayingCardHand
    {
        public IDictionary<PlayingCard, int> CardsByCount { get; set; }

        public void SetCards(IEnumerable<PlayingCard> playingCards)
        {
            CardsByCount = new Dictionary<PlayingCard, int>(new PlayingCardComparer());
            AddCards(playingCards);
        }

        public void AddCards(IEnumerable<PlayingCard> playingCards)
        {
            CardsByCount = CardsByCount ?? new Dictionary<PlayingCard, int>(new PlayingCardComparer());
            foreach (var playingCard in playingCards)
            {
                if (CardsByCount.ContainsKey(playingCard)) { CardsByCount[playingCard]++; }
                else { CardsByCount.Add(playingCard, 1); }
            }
        }
    }
}