using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace PokerHands.Solver
{
    public class Solver
    {
        public Dictionary<string, FiveCardStudScore> GetScoreByPlayer(IEnumerable<string> playersNames, IEnumerable<IEnumerable<string>> playersCards)
        {
            if (playersNames.Count() != playersCards.Count()) { throw new ArgumentException($"A mismatch between the number of players ({playersNames.Count()}) and the number of hands (${playersCards.Count()}) was detected.");  }
            var playingCardHandConverter = new PlayingCardHandConverter();
            var playersHands = playersCards.Select(cardStrings => playingCardHandConverter.Convert(cardStrings)).ToArray();
            var validationRules = new SingleHandPokerNoJokersValidationRules();
            if(!validationRules.IsValid(playersHands)) { throw new ArgumentException($"The provided card strings do not represent a valid combination of hands - {new JavaScriptSerializer().Serialize(playersCards)}");}
            return Enumerable.Range(0, playersNames.Count()).ToDictionary(i => playersNames.ElementAt(i), i => new FiveCardStudScore(playersHands[i]));
        }
    }
}