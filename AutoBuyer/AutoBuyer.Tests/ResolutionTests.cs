using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoBuyer.Tests
{
    [TestClass]
    public class ResolutionTests
    {
        [TestMethod]
        public void DoResolutionAdjustmentTest()
        {
            var resolution = Screen.PrimaryScreen.Bounds;
        }

        [TestMethod]
        public void CreateRandomFilterPrice()
        {
            const int maxPrice = 700;

            int currentMinBid = 0;
            int currentMinBuy = 0;

            for (int i = 0; i < 5; i++) // try to get a fresh filter set a max of 5 times, if it produces the same numbers 5 times, eh...
            {
                var randoFilterPrices = GetFilterPrices(maxPrice);

                if (randoFilterPrices.Item1 != currentMinBid && randoFilterPrices.Item2 != currentMinBuy)
                {
                    currentMinBid = randoFilterPrices.Item1;
                    currentMinBuy = randoFilterPrices.Item2;
                    break;
                }
            }
        }

        public Tuple<int, int> GetFilterPrices(int maxPrice)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            var next1 = random.Next(2, maxPrice / 100);
            var next2 = random.Next(2, maxPrice / 100);

            var halfOrNah = DateTime.Now.Ticks % 2 == 0;

            var first = next1 * 100;
            var second = next2 * 100;

            if (halfOrNah)
            {
                first += 50;
                second += 50;
            }

            var minBid = Math.Min(first, second);
            var minBuy = Math.Max(first, second);

            return new Tuple<int, int>(minBid, minBuy); 
        }
    }
}