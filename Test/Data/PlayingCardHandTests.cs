using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerHands.Data.Enums;

namespace PokerHands.Data.Tests
{
    [TestClass]
    public class PlayingCardHandTests
    {
        [TestMethod]
        public void SetCardsTest()
        {
            PlayingCard aceOfClubs = new PlayingCard(Suit.Clubs, CardValue.Ace);
            PlayingCard threeOfHearts = new PlayingCard(Suit.Hearts, CardValue.Three);
            PlayingCard[] playingCards = new [] { aceOfClubs, aceOfClubs, threeOfHearts };
            PlayingCardHand hand = new PlayingCardHand();

            Assert.IsNull(hand.CardsByCount);

            hand.SetCards(playingCards);
            Assert.AreEqual(2, hand.CardsByCount[aceOfClubs]);
            Assert.AreEqual(1, hand.CardsByCount[threeOfHearts]);

            PlayingCard threeOfHearts2 = new PlayingCard(Suit.Hearts, CardValue.Three);
            playingCards = new [] { threeOfHearts, threeOfHearts2 };
            hand.SetCards(playingCards);
            Assert.IsFalse(hand.CardsByCount.ContainsKey(aceOfClubs));
            Assert.AreEqual(2, hand.CardsByCount[threeOfHearts]);
        }

        [TestMethod]
        public void AddCardsTest()
        {
            PlayingCard aceOfClubs = new PlayingCard(Suit.Clubs, CardValue.Ace);
            PlayingCard threeOfHearts = new PlayingCard(Suit.Hearts, CardValue.Three);
            PlayingCard[] playingCards = new[] { aceOfClubs, aceOfClubs, threeOfHearts };
            PlayingCardHand hand = new PlayingCardHand();

            Assert.IsNull(hand.CardsByCount);

            hand.AddCards(playingCards);
            Assert.AreEqual(2, hand.CardsByCount[aceOfClubs]);
            Assert.AreEqual(1, hand.CardsByCount[threeOfHearts]);

            PlayingCard threeOfHearts2 = new PlayingCard(Suit.Hearts, CardValue.Three);

            playingCards = new[] { threeOfHearts, threeOfHearts2 };
            hand.AddCards(playingCards);
            Assert.AreEqual(2, hand.CardsByCount[aceOfClubs]);
            Assert.AreEqual(3, hand.CardsByCount[threeOfHearts]);
        }
    }
}