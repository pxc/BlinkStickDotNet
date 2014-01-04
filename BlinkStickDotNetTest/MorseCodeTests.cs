using System;
using System.Linq;
using NUnit.Framework;
using BlinkStickDotNet;

namespace BlinkStickDotNetTest
{
    [TestFixture]
    public class MorseCodeTests
    {
        [Test]
        public static void TestRelativeLengthsAreAllPositiveExpectTrue()
        {
            Assert.That(MorseCode.RelativeLengths.Values.All(v => v > 0), Is.True);
        }

        [Test]
        public static void TestEncodingSimpleAreAllDifferentApartFromKnownCollisionsExpectTrue()
        {
            var knownCollisions = new[] { 'æ', 'ą', 'å', 'ĉ', 'ć', 'ł', 'đ', 'ę', 'ĥ', 'ń', 'ø', 'ó', 'ŭ', };
            var valuesExceptKnownCollisions = MorseCode.EncodingSimple.Where(kvp => !knownCollisions.Contains(kvp.Key)).Select(kvp => kvp.Value);

            foreach (string collision in valuesExceptKnownCollisions.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToArray())
            {
                Console.WriteLine(collision);
            }

            Assert.That(valuesExceptKnownCollisions, Is.Unique);
        }

        [Test]
        public static void TestEncodingSimpleIsNotNullOrEmptyStringExpectTrue()
        {
            Assert.That(MorseCode.EncodingSimple.Values, Has.None.Null.And.None.Empty);
        }

        [Test]
        public static void TestEncodingSimpleContainsOnlyDotsAndDashesExpectTrue()
        {
            char[] dotdash = new[] { '.', '-' };
            Assert.That(MorseCode.EncodingSimple.Values.Select(es => es.Trim(dotdash)), Is.All.EqualTo(string.Empty));
        }

        [Test]
        public static void TestEncodeEmptyStringExpectEmptySequence()
        {
            Assert.That(MorseCode.Encode(""), Is.Empty);
        }

        [Test]
        public static void TestEncodeNullExpectEmptySequence()
        {
            Assert.That(MorseCode.Encode(null), Is.Empty);
        }

        [Test]
        public static void TestEncodeSimpleCharacterExpectSuccess()
        {
            var expectedResult = new[] {
                MorseCodeElement.Dot,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dash };

            Assert.That(MorseCode.Encode("U"), Is.EquivalentTo(expectedResult));
        }

        [Test]
        public static void TestEncodeSimpleWordExpectSuccess()
        {
            var expectedResult = new[] {
                MorseCodeElement.Dash,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dash,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dash,
                MorseCodeElement.InterLetterGap,
                MorseCodeElement.Dash,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dash
            };

            Assert.That(MorseCode.Encode("OK"), Is.EquivalentTo(expectedResult));
        }

        [Test]
        public static void TestEncodeSimpleSentenceExpectSuccess()
        {
            var expectedResult = new[] {
                MorseCodeElement.Dash, // Y
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dash,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dash,
                MorseCodeElement.InterLetterGap,
                MorseCodeElement.Dot, // E
                MorseCodeElement.InterLetterGap,
                MorseCodeElement.Dot, // S
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterWordGap, 
                MorseCodeElement.Dot, // S
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterLetterGap, 
                MorseCodeElement.Dot, // I
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dot,
                MorseCodeElement.InterLetterGap, 
                MorseCodeElement.Dot, // R
                MorseCodeElement.InterElementGap,
                MorseCodeElement.Dash,
                MorseCodeElement.InterElementGap, 
                MorseCodeElement.Dot
            };

            var result = MorseCode.Encode("YES SIR").ToArray();

            var length = Math.Max(result.Length, expectedResult.Length);
            for (int i = 0; i < length; i++)
            {
                string expected = i <= expectedResult.Length - 1 ? expectedResult[i].ToString() : "-";
                string actual = i <= result.Length - 1 ? result[i].ToString() : "-";
                Console.WriteLine("{0:G2} {1} {2}", i, expected, actual);
            }


            Assert.That(MorseCode.Encode("YES SIR"), Is.EquivalentTo(expectedResult));
        }
    }
}
