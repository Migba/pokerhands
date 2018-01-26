using System;
using System.Collections.Generic;
using System.Linq;
using PokerHands.Data;
using PokerHands.Data.Enums;
using PokerHands.Data.Interfaces;

namespace PokerHands.Solver
{
    public class  SingleHandPokerNoJokersValidationRules : IValidationRules<IEnumerable<PlayingCardHand>>
    {
        public bool IsValid(IEnumerable<PlayingCardHand> hands)
        {
            if (hands == null) { throw new ArgumentNullException(); }
            if (!hands.Any()) { throw new ArgumentException("No hands were passed."); }
            if (hands.Any(h => h.CardsByCount.Count != 5)) {  return false; }
            try
            {
                var drawnCards = new Dictionary<PlayingCard, int>(new PlayingCardComparer());
                foreach (var hand in hands)
                {
                    foreach (var countByCard in hand.CardsByCount)
                    {
                        if(drawnCards.ContainsKey(countByCard.Key) || countByCard.Value != 1) { throw new Exception("There should only be one of each card in a single deck."); }
                        drawnCards.Add(countByCard.Key, countByCard.Value);
                    }
                }

                if (drawnCards.Any(kvp => kvp.Key.Suit == Suit.JokerOne
                                       || kvp.Key.Suit == Suit.JokerTwo
                                       || kvp.Key.Suit == Suit.None
                                       || kvp.Key.CardValue == CardValue.AceLow
                                       || kvp.Key.CardValue == CardValue.Joker
                                       || kvp.Key.CardValue == CardValue.None))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }    
}