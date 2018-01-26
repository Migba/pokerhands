using System;
using System.Collections.Generic;
using System.Linq;
using PokerHands.Data;
using PokerHands.Data.Interfaces;

namespace PokerHands.Solver
{
    public class PlayingCardHandConverter : IConverter<IEnumerable<string>, PlayingCardHand>
    {
        private readonly PlayingCardConverter _playingCardConverter = new PlayingCardConverter();
        public PlayingCardHand Convert(IEnumerable<string> handAsCardStrings)
        {
            if (handAsCardStrings == null) { throw new ArgumentNullException(); }
            if (!handAsCardStrings.Any()) { throw new ArgumentException("No card strings were provided to the converter."); }

            var playingCards = handAsCardStrings.Select(s => _playingCardConverter.Convert(s));
            var playingCardHand = new PlayingCardHand();
            playingCardHand.SetCards(playingCards);

            return playingCardHand;
        }
    }
}