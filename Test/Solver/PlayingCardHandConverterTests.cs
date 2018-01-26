using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using PokerHands.Data;
using PokerHands.Data.Enums;

namespace PokerHands.Solver.Tests
{
    [TestClass]
    public class PlayingCardHandConverterTests
    {
        private readonly PlayingCardHandConverter _playingCardHandConverter = new PlayingCardHandConverter();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Convert_ThrowsArgumentNullExceptionWhenPassedNull()
        {
            PlayingCardHand playingCardHand = _playingCardHandConverter.Convert(null);
        }

        [TestMethod]
        public void Convert_ThrowsArgumentExceptionWhenPassedNull()
        {
            try
            {
                PlayingCardHand playingCardHand = _playingCardHandConverter.Convert(new List<string>());
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                Assert.AreEqual("No card strings were provided to the converter.", e.Message);
                return;
            }
            Assert.Fail("No Exception Was Thrown.");
        }

        [TestMethod]
        public void Convert_ConvertsHandSuccessfully()
        {
            string[] cardStrings = new[] { "2H", "3H", "4H", "5H", "6H" };
            PlayingCardHand playingCardHand = _playingCardHandConverter.Convert(cardStrings);
            PlayingCard twoOfHearts = new PlayingCard(Suit.Hearts, CardValue.Two);
            PlayingCard threeOfHearts = new PlayingCard(Suit.Hearts, CardValue.Three);
            PlayingCard fourOfHearts = new PlayingCard(Suit.Hearts, CardValue.Four);
            PlayingCard fiveOfHearts = new PlayingCard(Suit.Hearts, CardValue.Five);
            PlayingCard sixOfHearts = new PlayingCard(Suit.Hearts, CardValue.Six);
            Assert.IsTrue(playingCardHand.CardsByCount.ContainsKey(twoOfHearts));
            Assert.IsTrue(playingCardHand.CardsByCount.ContainsKey(threeOfHearts));
            Assert.IsTrue(playingCardHand.CardsByCount.ContainsKey(fourOfHearts));
            Assert.IsTrue(playingCardHand.CardsByCount.ContainsKey(fiveOfHearts));
            Assert.IsTrue(playingCardHand.CardsByCount.ContainsKey(sixOfHearts));
            Assert.IsTrue(playingCardHand.CardsByCount.Keys.Count == 5);
        }
    }
}