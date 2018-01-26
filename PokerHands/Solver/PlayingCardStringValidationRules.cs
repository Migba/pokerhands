using System;
using System.Linq;
using System.Text.RegularExpressions;
using PokerHands.Data.Interfaces;

namespace PokerHands.Solver
{
    class PlayingCardStringValidationRules : IValidationRules<string>
    {
        public bool IsValid(string cardString)
        {
            if (cardString == null) {  throw new ArgumentNullException(); } 
            var cardStringContains10 = cardString.Contains("10");
            if (cardString.Length != 2 && !(cardString.Length == 3 && cardStringContains10)) { return false; }
            Regex invalidCharactersRegex = new Regex("[^CDHS2345678910JQKA]");
            if (invalidCharactersRegex.IsMatch(cardString)) { return false; }
            var possibleSuitChars = new char[] {'C', 'D', 'H', 'S'};
            if (cardString.Count(c => possibleSuitChars.Contains(c)) != 1) {  return false; }
            Regex tenCharactersRegex = new Regex("[10]");
            if (!cardStringContains10 && tenCharactersRegex.IsMatch(cardString)) { return false; }
            return true;
        }
    }
}