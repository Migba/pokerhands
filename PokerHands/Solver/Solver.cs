using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerHands.Utilities;
using PokerHands.Data;
using System
namespace PokerHands.Solver
{
    public class Solver
    {
        /* Probabilities of Poker Hands
         * Straight Flush = 0.001544%
         * Four of a Kind = 0.0240%
         * Full House = 0.1441%
         * Flush = 0.1965%
         * Straight = 0.3925%
         * Three of a Kind = 2.1128%
         * Two Pair = 4.7539%
         * One Pair = 42.2569%
         * High Card = 50.1177%
         */

            //check for invalid hands, two aces of spades
    }

    public class Hand //: IHand
    {
        // We would use a multiset if C# ideally, but instead we'll use a Dictionary with the number of cards as the value. 
        // In the case of one-deck poker, this should always be one for a valid hand.
        public Dictionary<Card, int> cards { get; set; }
        public void setCards

        public IValidationRules<Hand> validationRules
        {
            get { return this.validationRules; }
            set
            {
                this._isValid = null;
                this.validationRules = value;
            }            
        }
        public bool IsValid
        {
            get { return _isValid ?? Validate(); }
        }
        private bool? _isValid = null;
        private bool Validate()
        {
            if (this.validationRules == null) { throw new NullReferenceException("No Validation Rules have been configured."); }
            _isValid = this.validationRules?.Validate(this) ?? false;
            return _isValid.Value;
        }
    }

    public interface IValidationRules<T>
    {
        bool Validate(T typeToValidate);
    }

    public class SingleHandPokerNoJokersValidationRules : IValidationRules<Hand>
    {
        public bool Validate(Hand hand)
        {
            if (hand.cards.Any(kvp => kvp.Value > 1)) { return false; }
            if (hand.cards.Any(kvp => kvp.Key.suit == Suit.JokerOne || kvp.Key.suit == Suit.JokerTwo)) { return false; }
            if (hand.cards.Any(kvp => kvp.Key.value == Value.Joker)) { return false; }
            return true;
        }
    }
    

    public class FiveCardStudScore : IComparable
    {

        /* Scoring Rules:
        * 
        * Straight Flush. Five cards of the same suit with consecutive values. Ranked by the highest card in the hand.
        * Four of a Kind. Four cards with the same value. Ranked by the value of the four cards.
        * Full House. Three cards of the same value, with the remaining two cards forming a pair. Ranked by the value of the three cards.  
        * Flush. Hand contains five cards of the same suit. Hands which are both flushes are ranked using the rules for High Card.
        * Straight. Hand contains five cards with consecutive values. Hands which both contain a straight are ranked by their highest card.
        * Three of a Kind. Three of the cards in the hand have the same value. Hands which both contain three of a kind are ranked by the value of the three cards.
        * Two Pairs: The hand contains two different pairs. Hands which both contain two pairs are ranked by the value of their highest pair. Hands with the same highest pair are ranked by the value of their other pair. If these values are the same the hands are ranked by the value of the remaining card.
        * Pair: Two of the five cards in the hand have the same value. Hands which both contain a pair are ranked by the value of the cards forming the pair. If these values are the same, the hands are ranked by the values of the cards not forming the pair, in decreasing order.
        * High Card: Hands which do not fit any higher category are ranked by the value of their highest card. If the highest cards have the same value, the hands are ranked by the next highest, and so on.
        */

        public Hand hand { get; private set; }
        public int[] rank { get; private set; }
        public HandType handType { get; private set; }
        public IValidationRules<ICollection<Hand>> handsValidationRules { get; set; }
        public IValidationRules<FiveCardStudScore> scoreValidationRules { get; set; }
        private Dictionary<Suit, int> countsBySuit;
        //private Dictionary<Value, int> valueCount;
        private Dictionary<int, HashSet<Value>> valuesByCount;

        private bool isAFlush => countsBySuit.First().Value == 5;
        //private bool isAThreeOfAKind => valueCount.Any(kvp => valueCount[kvp.Key] == 3);
        private bool isAThreeOfAKind => valuesByCount.ContainsKey(3);
        private bool isAFourOfAKind => valuesByCount.ContainsKey(4);
        private bool isTwoPair => valuesByCount.ContainsKey(2) && valuesByCount[2].Count == 2;
        private bool isOnePair => valuesByCount.ContainsKey(2) && valuesByCount[2].Count == 1;
        private bool isAStraight(IOrderedEnumerable<Card> cards)
        {
            int lastRank = -1;
            foreach (Card card in cards)
            {
                if ((int)card.value == lastRank - 1 || lastRank == -1)
                {
                    lastRank = (int)card.value;
                }
                else { return false; }
            }
            return true;
        }        

        public FiveCardStudScore(Hand hand)
        {
            if (!hand.IsValid) { throw new InvalidHandException("The hand being scored is invalid.");}
            this.hand = hand;
            var cards = hand.cards.Select(kvp => kvp.Key).OrderByDescending(c => c.value);
            Dictionary<Suit, int> suitCount = cards.ToDictionary(c => c.suit, c => cards.Count(c2 => c2.suit == c.suit));
            Dictionary<Value, int> valueCount = cards.ToDictionary(c => c.value, c => cards.Count(c2 => c2.value == c.value));

            //Dictionary<int, Value> valueCount2 = cards.ToDictionary(c => cards.Count(c2 => c2.value == c.value), c => c.value, );

            //Check for Four Of A Kind first as it is the easier to determine than a Straight Flush due to the Dictionary
            if (isAFourOfAKind)
            {
                handType = HandType.FourOfAKind;
                //int fourOfAKindRank = (int) valueCount.First(kvp => valueCount[kvp.Key] == 4).Key;
                int fourOfAKindRank = (int)valuesByCount[4].Single();
                //int leftoverRank = (int) valueCount.Single(kvp => valueCount[kvp.Key] == 1).Key;
                int leftoverRank = (int)valuesByCount[1].Single();
                rank = new int[] { fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, leftoverRank };
                //We can't possibly have a straight flush if we have four of a kind so there's no need to look for anything else
                return; 
            }
            bool threeOfAKindFound = false;
            int threeOfAKindRank = -1;
            //Next, check for Full House as we need to check for Three of a Kind first and that is easy to determine with the Dictionary
            //Although the Flush is easier to check for, 
            //Also, even if we don't have a Full House we can memoize the check for the Three of a Kind, giving us helpful information going forward
            if (isAThreeOfAKind)
            {                
                threeOfAKindFound = true;
                //threeOfAKindRank = (int) valueCount.First(kvp => valueCount[kvp.Key] == 3).Key;
                threeOfAKindRank = (int)valuesByCount[3].Single();
                //var remainingCardsValues = valueCount2.Select(kvp => kvp.Key).Where(v => (int) v != threeOfAKindRank);
                //if (remainingCardsValues.Count(v => v == remainingCardsValues.First()) == 2)
                if (valuesByCount.ContainsKey(2))
                {
                    //var remainingTwoOfAKindRank = (int) remainingCardsValues.First();
                    var remainingTwoOfAKindRank = (int)valuesByCount[2].Single();
                    handType = HandType.FullHouse;
                    rank = new int[] { threeOfAKindRank, threeOfAKindRank, threeOfAKindRank, remainingTwoOfAKindRank, remainingTwoOfAKindRank };
                    //We can't possibly have a straight flush if we have a full house so there's no need to look for anything else
                    return;
                }
            }
            //We check for a Flush next as it is also easy to determine using the dictionary
            //Also, we can avoid the iteration in isAStraight on the off-chance we also have a Three of a Kind
            if (isAFlush)
            {
                handType = HandType.Flush;
                rank = cards.Select(c => (int) c.value).ToArray();
                if (threeOfAKindFound)
                {
                    //We can't possibly have a straight flush if we have three of a kind so there's no need to check
                    return;
                }
                if(isAStraight(cards))
                {
                    handType = HandType.StraightFlush;
                    return;
                }
            }
            if (threeOfAKindFound)
            {
                handType = HandType.ThreeOfAKind;
                //var remainingCardsValues = valueCount.Select(kvp => (int) kvp.Key).Where(i => i != threeOfAKindRank).OrderByDescending(i => i).ToArray();
                var remainingCardsValues = valuesByCount[1].OrderByDescending(v => v).ToArray();
                rank = new int[] { threeOfAKindRank, threeOfAKindRank, threeOfAKindRank, (int)remainingCardsValues[0], (int)remainingCardsValues[1] };
                //We can't possibly have a straight if we have three of a kind so there's no need to check
                return;
            }
            //We'll check for Two Pair next, and we'll save our first found pair in case we only have One Pair
            if (isTwoPair)
            {
                int firstPairRank = (int)valuesByCount[2].First();
                int lastPairRank = (int)valuesByCount[2].Last();
                //int firstPairRank = (int) valueCount.First(kvp => valueCount[kvp.Key] == 2).Key;

                valueCount[(Value) firstPairRank] = -2;
                if (valueCount.Any(kvp => valueCount[kvp.Key] == 2))
                {
                    handType = HandType.TwoPair;
                    var highTwoOfAKindRank = firstPairRank > lastPairRank ? firstPairRank : lastPairRank;
                    var lowTwoOfAKindRank = highTwoOfAKindRank == firstPairRank ? lastPairRank : firstPairRank;
                    //var remainingCardRank = (int) cards.Single(c => ((int) c.value != firstPairRank && (int) c.value != lastPairRank)).value;
                    var remainingCardRank = (int)valuesByCount[1].Single();
                    rank = new int[] { highTwoOfAKindRank, highTwoOfAKindRank, lowTwoOfAKindRank, lowTwoOfAKindRank, remainingCardRank };
                    // We can't possibly have a straight if we have two pair so there's no need to check
                    return;
                }
            }
            if (isOnePair)
            {
                var onePairRank = (int)valuesByCount[2].Single();
                var remainingCardValues = valuesByCount[1].OrderByDescending(v => v).ToArray();
                rank = new int[] { onePairRank, onePairRank, (int)remainingCardValues[0], (int)remainingCardValues[1], (int)remainingCardValues[2] };
                // We can't possibly have a straight if we have a pair so there's no need to check
                return;
            }
            //Whether we have a straight or high-card we already know the ranking is the same
            rank = cards.Select(c => (int)c.value).ToArray();
            if (isAStraight(cards))
            {
                handType = HandType.Straight;                
                return;
            }
            handType = HandType.HighCard;
            return;
        }
        
        public int CompareTo(object o)
        {
            if (!this.hand.IsValid) { throw new InvalidHandException("The hand being scored in invalid.");  }
            if (o == null) { throw new ArgumentNullException();  }
            if (!(o is FiveCardStudScore)) { throw new ArgumentException($"Argument was of type - {o.GetType().ToString()}, expected type - {this.GetType().ToString()}");  }
            FiveCardStudScore comparedScore = o as FiveCardStudScore;
            if (!comparedScore.hand.IsValid) { throw new InvalidHandException("The hand being compared is invalid.");  }
            HashSet<Hand> otherHands = new HashSet<Hand>() { comparedScore.hand };
            if (!isValidCombinationOfHands(otherHands)) { throw new InvalidHandCombinationException("The two hands being compared are invalid together.");  }
            
            if (this.handType > comparedScore.handType) { return 1; }
            if (this.handType < comparedScore.handType) { return -1; }
            for (int i = 0; i < this.rank.Count(); i ++)
            {
                if (this.rank[i] > comparedScore.rank[i]) { return 1; }
                if (this.rank[i] < comparedScore.rank[i]) { return -1; }
            }
            return 0;
        }

        public bool isValidCombinationOfHands(ICollection<Hand> otherHands)
        {
            var allHands = otherHands;
            allHands.Add(this.hand);
            return handsValidationRules.Validate(allHands);
        }      
    }
    

    public enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }

    public class InvalidHandException : Exception
    {
        public InvalidHandException(String message) : base(message) { }
    }

    public class InvalidHandCombinationException : Exception
    {
        public InvalidHandCombinationException(String message) : base(message) { }
    }
}