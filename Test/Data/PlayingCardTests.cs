using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerHands.Data.Enums;

namespace PokerHands.Data.Tests
{
    [TestClass]
    public class PlayingCardTests
    {
        [TestMethod]
        public void PlayingCard_ConstructorTest()
        {
            PlayingCard playingCard = new PlayingCard(Suit.Clubs, CardValue.Ace);
            Assert.AreEqual(Suit.Clubs, playingCard.Suit);
            Assert.AreEqual(CardValue.Ace, playingCard.CardValue);
        }

    }
}