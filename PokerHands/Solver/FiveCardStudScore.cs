using System;
using System.Collections.Generic;
using System.Linq;
using PokerHands.Data;
using PokerHands.Data.Enums;

namespace PokerHands.Solver
{
    public class FiveCardStudScore : IComparable
    {
        /* Scoring Rules:
        * 
        * Straight Flush. Five cards of the same suit with consecutive values. Ranked by the highest card in the hand.
        * Four of a Kind. Four cards with the same cardValue. Ranked by the cardValue of the four cards.
        * Full House. Three cards of the same cardValue, with the remaining two cards forming a pair. Ranked by the cardValue of the three cards.  
        * Flush. Hand contains five cards of the same suit. Hands which are both flushes are ranked using the rules for High Card.
        * Straight. Hand contains five cards with consecutive values. Hands which both contain a straight are ranked by their highest card.
        * Three of a Kind. Three of the cards in the hand have the same cardValue. Hands which both contain three of a kind are ranked by the cardValue of the three cards.
        * Two Pairs: The hand contains two different pairs. Hands which both contain two pairs are ranked by the cardValue of their highest pair. Hands with the same highest pair are ranked by the cardValue of their other pair. If these values are the same the hands are ranked by the cardValue of the remaining card.
        * Pair: Two of the five cards in the hand have the same cardValue. Hands which both contain a pair are ranked by the cardValue of the cards forming the pair. If these values are the same, the hands are ranked by the values of the cards not forming the pair, in decreasing order.
        * High Card: Hands which do not fit any higher category are ranked by the cardValue of their highest card. If the highest cards have the same cardValue, the hands are ranked by the next highest, and so on.
        */

        private readonly Dictionary<Suit, int> _countsBySuit;
        private readonly Dictionary<int, HashSet<CardValue>> _valuesByCount;

        public PlayingCardHand Hand { get; }
        public int[] Rank { get; }
        public HandType HandType { get; }

        public FiveCardStudScore(PlayingCardHand hand)
        {
            if (!IsValid(hand)) { throw new ArgumentException("The hand being scored is invalid."); }
            _countsBySuit = new Dictionary<Suit, int>() { { Suit.Clubs, 0 }, { Suit.Diamonds, 0 }, { Suit.Hearts, 0 }, { Suit.Spades, 0 } };
            _valuesByCount = new Dictionary<int, HashSet<CardValue>>() { { 1, null }, { 2, null }, { 3, null }, { 4, null } };
            Hand = hand;
            var cards = hand.CardsByCount.Select(kvp => kvp.Key).OrderByDescending(c => c.CardValue);
            foreach (var suit in _countsBySuit.Keys.ToList())
            {
                _countsBySuit[suit] = cards.Count(c => c.Suit == suit);
            }
            foreach (var count in _valuesByCount.Keys.ToList())
            {
                var values = new HashSet<CardValue>();
                values.UnionWith(cards.Select(c => c.CardValue).Where(v => cards.Count(c => c.CardValue == v) == count));
                _valuesByCount[count] = values;
            }

            //Check for Four Of A Kind first as it is the easier to determine than a Straight Flush due to the Dictionary
            if (IsAFourOfAKind)
            {
                HandType = HandType.FourOfAKind;
                int fourOfAKindRank = (int)_valuesByCount[4].Single();
                int leftoverRank = (int)_valuesByCount[1].Single();
                Rank = new int[] { fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, leftoverRank };
                //We can't possibly have a straight flush if we have four of a kind so there's no need to look for anything else
                return; 
            }
            bool threeOfAKindFound = false;
            int threeOfAKindRank = -1;
            //Next, check for Full House as we need to check for Three of a Kind first and that is easy to determine with the Dictionary
            //Although the Flush is easier to check for, 
            //Also, even if we don't have a Full House we can memoize the check for the Three of a Kind, giving us helpful information going forward
            if (IsAThreeOfAKind)
            {                
                threeOfAKindFound = true;
                threeOfAKindRank = (int)_valuesByCount[3].Single();
                if (_valuesByCount[2].Any())
                {
                    var remainingTwoOfAKindRank = (int)_valuesByCount[2].Single();
                    HandType = HandType.FullHouse;
                    Rank = new int[] { threeOfAKindRank, threeOfAKindRank, threeOfAKindRank, remainingTwoOfAKindRank, remainingTwoOfAKindRank };
                    //We can't possibly have a straight flush if we have a full house so there's no need to look for anything else
                    return;
                }
            }
            //We check for a Flush next as it is also easy to determine using the dictionary
            //Also, we can avoid the iteration in isAStraight on the off-chance we also have a Three of a Kind
            if (IsAFlush)
            {
                HandType = HandType.Flush;
                Rank = cards.Select(c => (int) c.CardValue).ToArray();
                if (threeOfAKindFound)
                {
                    //We can't possibly have a straight flush if we have three of a kind so there's no need to check
                    return;
                }
                if(IsAStraight(cards))
                {
                    HandType = HandType.StraightFlush;
                    return;
                }
                return;
            }
            if (threeOfAKindFound)
            {
                HandType = HandType.ThreeOfAKind;
                var remainingCardsValues = _valuesByCount[1].OrderByDescending(v => v).ToArray();
                Rank = new int[] { threeOfAKindRank, threeOfAKindRank, threeOfAKindRank, (int)remainingCardsValues[0], (int)remainingCardsValues[1] };
                //We can't possibly have a straight if we have three of a kind so there's no need to check
                return;
            }
            //We'll check for Two Pair next, and we'll save our first found pair in case we only have One Pair
            if (IsTwoPair)
            {
                HandType = HandType.TwoPair;
                int firstPairRank = (int)_valuesByCount[2].First();
                int lastPairRank = (int)_valuesByCount[2].Last();
                var highTwoOfAKindRank = firstPairRank > lastPairRank ? firstPairRank : lastPairRank;
                var lowTwoOfAKindRank = highTwoOfAKindRank == firstPairRank ? lastPairRank : firstPairRank;
                var remainingCardRank = (int)_valuesByCount[1].Single();
                Rank = new int[] { highTwoOfAKindRank, highTwoOfAKindRank, lowTwoOfAKindRank, lowTwoOfAKindRank, remainingCardRank };
                // We can't possibly have a straight if we have two pair so there's no need to check
                return;
            }
            if (IsOnePair)
            {
                var onePairRank = (int)_valuesByCount[2].Single();
                var remainingCardValues = _valuesByCount[1].OrderByDescending(v => v).ToArray();
                Rank = new int[] { onePairRank, onePairRank, (int)remainingCardValues[0], (int)remainingCardValues[1], (int)remainingCardValues[2] };
                HandType = HandType.OnePair;
                // We can't possibly have a straight if we have a pair so there's no need to check
                return;
            }
            //Whether we have a straight or high-card we already know the ranking is the same (unless it's an Ace-Low Straight
            Rank = cards.Select(c => (int)c.CardValue).ToArray();
            if (IsAStraight(cards))
            {
                HandType = HandType.Straight;                
                return;
            }
            //Check for Ace-Low Straight
            if (cards.Any(pc => pc.CardValue == CardValue.Ace))
            {
                var aceLowHand = cards.Select(pc => pc.CardValue == CardValue.Ace ? new PlayingCard(pc.Suit, CardValue.AceLow) : pc).OrderByDescending(pc => pc.CardValue);
                if (IsAStraight(aceLowHand))
                {
                    HandType = HandType.Straight;
                    Rank = aceLowHand.Select(c => (int) c.CardValue).ToArray();
                    return;
                }
            }
            HandType = HandType.HighCard;
            return;
        }

        public int CompareTo(object comparedScoreObject)
        {
            if (comparedScoreObject == null) { throw new ArgumentNullException();  }
            if (!(comparedScoreObject is FiveCardStudScore)) { throw new ArgumentException($"Argument was of type - {comparedScoreObject.GetType().ToString()}, expected type - {GetType().ToString()}");  }
            FiveCardStudScore comparedScore = (FiveCardStudScore) comparedScoreObject;
            HashSet<PlayingCardHand> otherHands = new HashSet<PlayingCardHand>() { comparedScore.Hand };
            if (HandType > comparedScore.HandType) { return 1; }
            if (HandType < comparedScore.HandType) { return -1; }
            for (int i = 0; i < Rank.Count(); i ++)
            {
                if (Rank[i] > comparedScore.Rank[i]) { return 1; }
                if (Rank[i] < comparedScore.Rank[i]) { return -1; }
            }
            return 0;
        }

        private bool IsAFlush => _countsBySuit.Any(kvp => kvp.Value == 5);

        private bool IsAThreeOfAKind => _valuesByCount[3] != null && _valuesByCount[3].Any();

        private bool IsAFourOfAKind => _valuesByCount[4] != null && _valuesByCount[4].Any();

        private bool IsTwoPair => _valuesByCount[2] != null && _valuesByCount[2].Count == 2;

        private bool IsOnePair => _valuesByCount[2] != null && _valuesByCount[2].Count == 1;

        private static bool IsAStraight(IEnumerable<PlayingCard> cards)
        {
            int lastRank = -1;
            foreach (PlayingCard card in cards)
            {
                if ((int)card.CardValue == lastRank - 1 || lastRank == -1)
                {
                    lastRank = (int)card.CardValue;
                }
                else { return false; }
            }
            return true;
        }

        private static bool IsValid(PlayingCardHand hand)
        {
            if (hand.CardsByCount.Any(kvp => kvp.Key.Suit == Suit.JokerOne
                                             || kvp.Key.Suit == Suit.JokerTwo
                                             || kvp.Key.Suit == Suit.None
                                             || kvp.Key.CardValue == CardValue.AceLow
                                             || kvp.Key.CardValue == CardValue.Joker
                                             || kvp.Key.CardValue == CardValue.None))
            {
                return false;
            }
            if (hand.CardsByCount.Sum(kvp => kvp.Value) != 5) { return false; }
            return true;
        }
    }
}