using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    public class BowlingScore
    {
        private const int TotalNumberOfPins = 10;
        private const int NumberOfFrames = 10;

        List<int> _rollScores = new List<int>();
        private int indexOfFirstRollInNextFrame;

        public void Roll(int numberOfPinsKnockedDown)
        {
            _rollScores.Add(numberOfPinsKnockedDown);
            
        }

        public int CalculateScore()
        {
            var totalScore = 0;
            var strikes = 0;

            for (var frameNumber = 0; frameNumber < NumberOfFrames; frameNumber++)
            {
                var indexOfFirstRollInFrame = frameNumber * 2 - strikes;

                var firstRollInFrame = _rollScores[indexOfFirstRollInFrame];

                int scoreFortheFrame;

                if (firstRollInFrame == TotalNumberOfPins)
                {
                    //strike
                    strikes++;
                    indexOfFirstRollInNextFrame = indexOfFirstRollInFrame + 1;
                    var indexOfSecondRollInNextFrame = indexOfFirstRollInFrame + 2;

                    scoreFortheFrame = TotalNumberOfPins 
                        + _rollScores[indexOfFirstRollInNextFrame] 
                        + _rollScores[indexOfSecondRollInNextFrame];
                }
                else
                {
                    var indexOfSecondRollInTheFrame = indexOfFirstRollInFrame + 1;
                    if (_rollScores[indexOfFirstRollInFrame] + _rollScores[indexOfSecondRollInTheFrame] == TotalNumberOfPins)
                    {
                        //spare
                        var indexOfFirstRolInNextFrame = indexOfFirstRollInFrame + 2;
                        scoreFortheFrame = TotalNumberOfPins + _rollScores[indexOfFirstRolInNextFrame];
                    }
                    else
                    {
                        scoreFortheFrame = _rollScores[indexOfFirstRollInFrame] + _rollScores[indexOfSecondRollInTheFrame];
                    }
                }

                totalScore += scoreFortheFrame;
            }

            return totalScore;
        }

        public int CalculateScoreNEW()
        {
            var totalScore = 0;
            for (var rollNumber = 0; rollNumber < _rollScores.Count; rollNumber++)
            {
                totalScore += _rollScores[rollNumber];

                if (rollNumber > 0 && _rollScores[rollNumber -1] == 10)
                    totalScore += _rollScores[rollNumber];

                else if (rollNumber > 1 && _rollScores[rollNumber - 1] + _rollScores[rollNumber - 2] > 10)
                    totalScore += _rollScores[rollNumber];
            }

            return totalScore;
        }
    }

    [TestFixture]
    public class BowlingTests
    {
        [Test]
        public void AllGutterBalls()
        {
            var scorer = new BowlingScore();

            for (var frame = 0; frame < 10; frame++)
            {
                scorer.Roll(0);
                scorer.Roll(0);
            }

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(0));
        }

        [Test]
        public void SinglePins()
        {
            var scorer = new BowlingScore();

            for (var frame = 0; frame < 10; frame++)
            {
                scorer.Roll(1);
                scorer.Roll(1);
            }

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(20));
        }


        [Test]
        public void FivePinsEveryTime()
        {
            var scorer = new BowlingScore();

            for (var frame = 0; frame < 10; frame++)
            {
                scorer.Roll(5);
                scorer.Roll(5);
            }

            //Spare roll
            scorer.Roll(5);

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(150));
        }

        [Test]
        public void MixedSparesEveryTime()
        {
            var scorer = new BowlingScore();

            for (var frame = 0; frame < 10; frame++)
            {
                scorer.Roll(3);
                scorer.Roll(7);
            }

            //Spare roll
            scorer.Roll(5);

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(13*9 + 15));
        }

        [Test]
        public void MixedSparesEveryTimeWithGutterFirst()
        {
            var scorer = new BowlingScore();

            for (var frame = 0; frame < 10; frame++)
            {
                scorer.Roll(0);
                scorer.Roll(10);
            }

            //Spare roll
            scorer.Roll(7);

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(10 * 9 + 17));
        }

        [Test]
        public void NonSpareFirstThenSpares()
        {
            var scorer = new BowlingScore();

            scorer.Roll(5);
            scorer.Roll(3);

            for (var frame = 0; frame < 9; frame++)
            {
                scorer.Roll(5);
                scorer.Roll(5);
            }

            //Spare roll
            scorer.Roll(7);

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(8 + (8 * 15) + 17));
        }

        [Test]
        public void SparesExceptLastFrame()
        {
            var scorer = new BowlingScore();
            
            for (var frame = 0; frame < 9; frame++)
            {
                scorer.Roll(5);
                scorer.Roll(5);
            }

            scorer.Roll(3);
            scorer.Roll(5);

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo((8 * 15) + 13 + 8));
        }

        [Test]
        public void NonSpareInMiddle()
        {
            var scorer = new BowlingScore();

            for (var frame = 0; frame < 4; frame++)
            {
                scorer.Roll(5);
                scorer.Roll(5);
            }

            scorer.Roll(3);
            scorer.Roll(5);

            for (var frame = 0; frame < 5; frame++)
            {
                scorer.Roll(5);
                scorer.Roll(5);
            }

            //Spare roll
            scorer.Roll(7);

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(3 * 15 + 13 + 8 + 4*15 + 17));
        }

        [Test]
        public void StrikeThenGutters()
        {
            var scorer = new BowlingScore();

            //strike
            scorer.Roll(10);

            for (var frame = 0; frame < 9; frame++)
            {
                scorer.Roll(0);
                scorer.Roll(0);
            }

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(10));
        }

        [Test]
        public void StrikeThenSinglePins()
        {
            var scorer = new BowlingScore();

            //strike
            scorer.Roll(10);

            for (var frame = 0; frame < 9; frame++)
            {
                scorer.Roll(1);
                scorer.Roll(1);
            }

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(12 + 9 * 2));
        }

        [Test]
        public void AllStrikes()
        {
            var scorer = new BowlingScore();
            
            for (var frame = 0; frame < 10; frame++)
            {
                scorer.Roll(10);
            }

            // strike bonus rolls
            scorer.Roll(10);
            scorer.Roll(10);

            var finalScore = scorer.CalculateScore();

            Assert.That(finalScore, Is.EqualTo(300));
        }

    }
}
