using System;
using PokerHands.Data;
using PokerHands.Data.Enums;
using PokerHands.Data.Interfaces;

namespace PokerHands.Solver
{
    public class PlayingCardConverter : IConverter<string, PlayingCard>
    {
        private readonly IValidationRules<string> _playingCardStringValidationRules = new PlayingCardStringValidationRules();
        public PlayingCard Convert(string cardString)
        {
            cardString = cardString.ToUpper().Trim();
            if(!_playingCardStringValidationRules.IsValid(cardString)) {  throw new ArgumentException($"The card string ({cardString}) is invalid."); }
            CardValue cardValue = CardValue.None;
            Suit suit = Suit.None;
            foreach (char c in cardString)
            {
                if (suit == Suit.None)
                {
                    var currentCharSuit = ParseSuit(c);
                    if (currentCharSuit != Suit.None)
                    {
                        suit = currentCharSuit;
                        continue;
                    }
                }
                if (cardValue == CardValue.None)
                {
                    var currentCharValue = ParseValue(c);
                    cardValue = currentCharValue;
                }

            }
            return new PlayingCard(suit, cardValue);
        }

        private static Suit ParseSuit(char c)
        {
            switch(c)
            {
                case 'C': return Suit.Clubs;
                case 'D': return Suit.Diamonds;
                case 'H': return Suit.Hearts;
                case 'S': return Suit.Spades;
                default: return Suit.None;
            }            
        }

        private static CardValue ParseValue(char c)
        {
            switch (c)
            {
                case '2': return CardValue.Two;
                case '3': return CardValue.Three;
                case '4': return CardValue.Four;
                case '5': return CardValue.Five;
                case '6': return CardValue.Six;
                case '7': return CardValue.Seven;
                case '8': return CardValue.Eight;
                case '9': return CardValue.Nine;
                case '1': return CardValue.Ten;
                case '0': return CardValue.Ten;
                case 'J': return CardValue.Jack;
                case 'Q': return CardValue.Queen;
                case 'K': return CardValue.King;
                case 'A': return CardValue.Ace;
                default: return CardValue.None;
            }
        }
    }
}