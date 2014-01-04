using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using NUnit.Framework;
using BlinkStickDotNet;

namespace BlinkStickDotNetTest
{
    [TestFixture]
    class BlinkStickTests
    {
        [Test]
        public static void TestLedColorWhenOnExpectSameAsSet()
        {
            Color expectedResult = Color.Red;
            Color result;

            using (var stick = new BlinkStick())
            {
                stick.LedColor = Color.Red;
                result = stick.LedColor;
            }

            Assert.That(result, Is.EqualTo(expectedResult).Using(s_saneColorComparer));
        }

        [Test]
        public static void TestLedColorWhenBlinkingExpectSameAsSet()
        {
            Color expectedResult = Color.White;
            Color result;

            using (var stick = new BlinkStick())
            {
                stick.Blink(Color.White, 300);
                Thread.Sleep(200);
                result = stick.LedColor;
            }

            Assert.That(result, Is.EqualTo(expectedResult).Using(s_saneColorComparer));
        }

        [Test]
        public static void TestLedColorIsResetAfterBlinkingExpectSuccess()
        {
            Color expectedResult = Color.Black;
            Color result;

            using (var stick = new BlinkStick())
            {
                stick.Blink(Color.White, 100);
                Thread.Sleep(200);
                result = stick.LedColor;                
            }

            Assert.That(result, Is.EqualTo(expectedResult).Using(s_saneColorComparer));
        }

        [Test]
        public static void TestBrightnessLimitExpectSuccess()
        {
            Color expectedResult = Color.FromArgb(112, 56, 15); // empirically determined
            Color result;

            using (var stick = new BlinkStick())
            {
                stick.SetBrightnessLimit(BlinkStick.Brightness.Brightish);
                stick.Blink(Color.SaddleBrown, 100);
                result = stick.LedColor;
            }

            Assert.That(result, Is.EqualTo(expectedResult).Using(s_saneColorComparer));
        }


        private static readonly SaneColorComparer s_saneColorComparer = new SaneColorComparer();

// ReSharper seems to think System.Drawing.Color.Equals is ambiguous
#pragma warning disable 1574
        /// <summary>
        /// <see cref="System.Drawing.Color.Equals"/> returns false for Red vs. (255, 0, 0) because one is a known color!
        /// </summary>
        private class SaneColorComparer : IComparer<Color>
        {
            public int Compare(Color x, Color y)
            {
                return x.ToArgb() - y.ToArgb();
            }
        }
#pragma warning restore 1574
    }
}
