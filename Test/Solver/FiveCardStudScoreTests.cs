using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using PokerHands.Data;
using PokerHands.Data.Enums;

namespace PokerHands.Solver.Tests
{
    [TestClass]
    public class FiveCardStudScoreTests
    {
        [TestMethod]
        public void FiveCardStudScoreTest_HighCard()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new []
            {
                new PlayingCard(Suit.Clubs, CardValue.Ace),
                new PlayingCard(Suit.Diamonds, CardValue.Four),
                new PlayingCard(Suit.Hearts, CardValue.Eight),
                new PlayingCard(Suit.Hearts, CardValue.Two), 
                new PlayingCard(Suit.Spades, CardValue.Three)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.Eight,
                (int) CardValue.Four,
                (int) CardValue.Three,
                (int) CardValue.Two,
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.HighCard, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_OnePair()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Ace),
                new PlayingCard(Suit.Diamonds, CardValue.Four),
                new PlayingCard(Suit.Hearts, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Two),
                new PlayingCard(Suit.Spades, CardValue.Three)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Four,
                (int) CardValue.Three,
                (int) CardValue.Two,
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.OnePair, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_TwoPair()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Ace),
                new PlayingCard(Suit.Diamonds, CardValue.Four),
                new PlayingCard(Suit.Hearts, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Two),
                new PlayingCard(Suit.Spades, CardValue.Four)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Four,
                (int) CardValue.Four,
                (int) CardValue.Two,
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.TwoPair, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_ThreeOfAKind()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Ace),
                new PlayingCard(Suit.Diamonds, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Two),
                new PlayingCard(Suit.Spades, CardValue.Three)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Three,
                (int) CardValue.Two,
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.ThreeOfAKind, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_AceLowStraight()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Ace),
                new PlayingCard(Suit.Diamonds, CardValue.Four),
                new PlayingCard(Suit.Hearts, CardValue.Five),
                new PlayingCard(Suit.Hearts, CardValue.Two),
                new PlayingCard(Suit.Spades, CardValue.Three)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Five,
                (int) CardValue.Four,
                (int) CardValue.Three,
                (int) CardValue.Two,
                (int) CardValue.AceLow
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.Straight, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_Straight()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Ten),
                new PlayingCard(Suit.Diamonds, CardValue.King),
                new PlayingCard(Suit.Hearts, CardValue.Queen),
                new PlayingCard(Suit.Hearts, CardValue.Jack),
                new PlayingCard(Suit.Spades, CardValue.Ace)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.King,
                (int) CardValue.Queen,
                (int) CardValue.Jack,
                (int) CardValue.Ten
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.Straight, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_Flush()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Nine),
                new PlayingCard(Suit.Clubs, CardValue.King),
                new PlayingCard(Suit.Clubs, CardValue.Queen),
                new PlayingCard(Suit.Clubs, CardValue.Jack),
                new PlayingCard(Suit.Clubs, CardValue.Ace)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.King,
                (int) CardValue.Queen,
                (int) CardValue.Jack,
                (int) CardValue.Nine
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.Flush, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_FullHouse()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Ace),
                new PlayingCard(Suit.Diamonds, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Two),
                new PlayingCard(Suit.Spades, CardValue.Two)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Two,
                (int) CardValue.Two,
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.FullHouse, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_FourOfAKind()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Clubs, CardValue.Ace),
                new PlayingCard(Suit.Diamonds, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Ace),
                new PlayingCard(Suit.Hearts, CardValue.Two),
                new PlayingCard(Suit.Spades, CardValue.Ace)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Ace,
                (int) CardValue.Two,
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.FourOfAKind, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_StraightFlush()
        {
            var hand = new PlayingCardHand();
            hand.SetCards(new[]
            {
                new PlayingCard(Suit.Hearts, CardValue.Ten),
                new PlayingCard(Suit.Hearts, CardValue.King),
                new PlayingCard(Suit.Hearts, CardValue.Queen),
                new PlayingCard(Suit.Hearts, CardValue.Jack),
                new PlayingCard(Suit.Hearts, CardValue.Ace)
            });
            var expectedRank = new int[]
            {
                (int) CardValue.Ace,
                (int) CardValue.King,
                (int) CardValue.Queen,
                (int) CardValue.Jack,
                (int) CardValue.Ten
            };

            var score = new FiveCardStudScore(hand);

            Assert.AreEqual(HandType.StraightFlush, score.HandType);
            Assert.IsTrue(expectedRank.SequenceEqual(score.Rank));
        }

        [TestMethod]
        public void FiveCardStudScoreTest_InvalidHand()
        {
            var invalidCardValues = new CardValue[] {CardValue.AceLow, CardValue.None, CardValue.Joker};
            var invalidSuits = new Suit[] {Suit.None, Suit.JokerOne, Suit.JokerTwo};

            var hand = new PlayingCardHand();
            for (var i = 0; i < invalidCardValues.Length; i++)
            {
                try
                {
                    hand.SetCards(new[]
                    {
                        new PlayingCard(Suit.Hearts, CardValue.Ten),
                        new PlayingCard(Suit.Hearts, CardValue.King),
                        new PlayingCard(Suit.Hearts, CardValue.Queen),
                        new PlayingCard(Suit.Hearts, CardValue.Jack),
                        new PlayingCard(Suit.Hearts, invalidCardValues[i])
                    });
                    var score = new FiveCardStudScore(hand);
                }
                catch (Exception e)
                {
                    continue;
                }
                Assert.Fail("Expected Exception Not Thrown!");
            }

            for (var i = 0; i < invalidSuits.Length; i++)
            {
                try
                {
                    hand.SetCards(new[]
                    {
                        new PlayingCard(Suit.Hearts, CardValue.Ten),
                        new PlayingCard(Suit.Hearts, CardValue.King),
                        new PlayingCard(Suit.Hearts, CardValue.Queen),
                        new PlayingCard(Suit.Hearts, CardValue.Jack),
                        new PlayingCard(invalidSuits[i], CardValue.Ace)
                    });
                    var score = new FiveCardStudScore(hand);
                }
                catch (Exception e)
                {
                    continue;
                }
                Assert.Fail("Expected Exception Not Thrown!");
            }
        }
    }
}