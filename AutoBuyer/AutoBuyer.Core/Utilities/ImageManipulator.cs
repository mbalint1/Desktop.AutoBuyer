using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AutoBuyer.Core.Utilities
{
    public class ImageManipulator
    {
        #region Enums

        public enum MatchAccuracy
        {
            Exact,
            HighProbability,
            MediumProbability,
            LowProbability
        }

        public enum CardType
        {
            RareGold,
            Gold,
            RareSilver,
            Silver
        }

        #endregion Enums

        #region Public Methods

        public bool ContainsPlayerCard(Image img, CardType cardType = CardType.RareGold)
        {
            var hexCodes = new List<string>();
            var bmpMin = new Bitmap(img, new Size(64, 64));

            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    var pixel = bmpMin.GetPixel(i, j);
                    var hex = $"{pixel.R:X2}{pixel.G:X2}{pixel.B:X2}";
                    hexCodes.Add(hex);
                }
            }

            var sorted = hexCodes.GroupBy(x => x).OrderByDescending(grp => grp.Count()).Take(10).Select(x => x.Key)
                .ToList();

            return false;
        }

        public Bitmap MakeGrayscale(Bitmap original)
        {
            var newBitmap = new Bitmap(original.Width, original.Height);

            using (var g = Graphics.FromImage(newBitmap))
            {
                var colorMatrix = new ColorMatrix(
                    new[]
                    {
                        new float[] {.3f, .3f, .3f, 0, 0},
                        new float[] {.59f, .59f, .59f, 0, 0},
                        new float[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });

                var attributes = new ImageAttributes();

                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return newBitmap;
        }

        //public bool IsMatch(Bitmap img1, Bitmap img2, MatchAccuracy accuracy, int totalPixelsInEach = 4096)
        //{
        //    var hash1 = GetHash(img1);
        //    var hash2 = GetHash(img2);

        //    var equalElements = hash1.Zip(hash2, (i, j) => i == j).Count(eq => eq);

        //    switch (accuracy)
        //    {
        //        case MatchAccuracy.Exact:
        //            return equalElements == totalPixelsInEach;
        //        case MatchAccuracy.HighProbability:
        //            return equalElements >= totalPixelsInEach * .9;
        //        case MatchAccuracy.MediumProbability:
        //            return equalElements >= totalPixelsInEach * .75;
        //        case MatchAccuracy.LowProbability:
        //            return equalElements < totalPixelsInEach * .75;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(accuracy), accuracy, null);
        //    }
        //}

        public decimal PercentMatch(Bitmap img1, Bitmap img2, int totalPixelsInEach = 4096)
        {
            var size = (int)Math.Sqrt(totalPixelsInEach);
            var hash1 = GetHash(img1, size);
            var hash2 = GetHash(img2, size);

            var equalElements = hash1.Zip(hash2, (i, j) => i == j).Count(eq => eq);

            return ((decimal)equalElements / (decimal)totalPixelsInEach) * 100;
        }

        public bool ContainsRed(Bitmap img)
        {
            // Pure red = 255,0,0 take the difference between RGB and average. Off red example 200,100,50 average diff of 68. Closer to 0 is closer to pure red.

            const int differenceThreshold = 90;
            const int numberOfRedPixelsThreshold = 10; // Seems arbitrary but works

            var redPixels = new List<int>();

            using (var bmap = new Bitmap(img, new Size(32, 32)))
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    for (int i = 0; i < bmap.Width; i++)
                    {
                        var pixel = bmap.GetPixel(i, j);
                        var r = pixel.R;
                        var g = pixel.G;
                        var b = pixel.B;

                        var rDiff = 255 - r;
                        var avgDiff = (g + b + rDiff) / 3;

                        if (avgDiff <= differenceThreshold)
                        {
                            redPixels.Add(avgDiff);
                        }
                    }
                }
            }

            return redPixels.Count >= numberOfRedPixelsThreshold;
        }

        public bool ContainsGreen(Bitmap img)
        {
            // Pure green = 0,255,0 take the difference between RGB and average. Off green example 100,200,50 average diff of 68. Closer to 0 is closer to pure green.

            const int differenceThreshold = 90;
            const int numberOfGreenPixelsThreshold = 10; // Seems arbitrary but works

            var greenPixels = new List<int>();

            using (var bmap = new Bitmap(img, new Size(32, 32)))
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    for (int i = 0; i < bmap.Width; i++)
                    {
                        var pixel = bmap.GetPixel(i, j);
                        var r = pixel.R;
                        var g = pixel.G;
                        var b = pixel.B;

                        var gDiff = 255 - g;
                        var avgDiff = (r + b + gDiff) / 3;

                        if (avgDiff <= differenceThreshold)
                        {
                            greenPixels.Add(avgDiff);
                        }
                    }
                }
            }

            return greenPixels.Count >= numberOfGreenPixelsThreshold;
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<bool> GetHash(Image bmpSource, int size)
        {
            var lResult = new List<bool>();

            using (var bmpMin = new Bitmap(bmpSource, new Size(size, size)))
            {
                for (int j = 0; j < bmpMin.Height; j++)
                {
                    for (int i = 0; i < bmpMin.Width; i++)
                    {
                        lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f); // 0.0 - 1.0 range (0.0 = black) (1.0 = white)
                    }
                }
                return lResult;

            }
        }

        #endregion Private Methods
    }
}