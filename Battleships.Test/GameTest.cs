using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Battleships;
using FluentAssertions;
using Xunit;

namespace Battleships.Test
{
    public class GameTest
    {
        [Fact]
        public void TestPlay()
        {
            var ships = new[] { "3:2,3:5" };
            var guesses = new[] { "7:0", "3:3" };
            Game.Play(ships, guesses).Should().Be(0);
        }

        //[Fact]
        //public void TestHit()
        //{
        //    var ships = new[] { "3:2,3:5" };
        //    var guesses = new[] { "7:0", "3:3" };
        //    Game.Play(ships, guesses).Should().Be(0);
        //    Game.Hits.Should().Be(1);
        //}
        [Fact]
        public void TestInvalidRangeThrowsException()
        {
            var ships = new[] { "3:5,3:2" };
            var guesses = new[] { "7:0", "3:3" };
            Assert.Throws<ArgumentException>(() => Game.Play(ships, guesses));
        }
        [Fact]
        public void TestSizeIsZeroThrowsException()
        {
            var ships = new[] { "3:5,3:5" };
            var guesses = new[] { "7:0", "3:3" };
            Assert.Throws<ArgumentException>(() => Game.Play(ships, guesses));
        }
        [Fact]
        public void TestSizeIsLessThanTwoThrowsException()
        {
            var ships = new[] { "3:5,3:4" };
            var guesses = new[] { "7:0", "3:3" };
            Assert.Throws<ArgumentException>(() => Game.Play(ships, guesses));
        }
        [Fact]
        public void TestIntersectingShipsThrowsException()
        {
            var ships = new[] { "3:5,3:2", "4:3,6:3" };
            var guesses = new[] { "7:0", "3:3" };
            Assert.Throws<ArgumentException>(() => Game.Play(ships, guesses));
        }
        [Fact]
        public void TestSinkingShipWithHorizontalRange()
        {
            var ships = new[] { "3:2,3:5" };
            var guesses = new[] { "7:0", "3:2", "3:3", "3:4", "3:5" };
            Game.Play(ships, guesses).Should().Be(1);
        }
        [Fact]
        public void TestSinkingShipWithVerticalRange()
        {
            var ships = new[] { "2:3,5:3" };
            var guesses = new[] { "7:0", "2:3", "3:3", "4:3", "5:3" };
            Game.Play(ships, guesses).Should().Be(1);
        }

    }
}
