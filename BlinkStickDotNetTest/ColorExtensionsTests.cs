using System.Drawing;
using BlinkStickDotNet;
using NUnit.Framework;

namespace BlinkStickDotNetTest
{
    [TestFixture]
    internal static class ColorExtensionsTests
    {
        private static readonly object[] FromAhsbTestCases =
        {
            new object[] { 104, 242, 0.1166362f, 0.9433382f, Color.FromArgb(104, 239, 239, 242) },
            new object[] { 156, 65, 0.4494953f, 0.04939589f, Color.FromArgb(156, 17, 18, 7) },
            new object[] { 165, 240, 0.1343008f, 0.8686357f, Color.FromArgb(165, 217, 217, 226) },
            new object[] { 58, 58, 0.9584425f, 0.1558313f, Color.FromArgb(58, 78, 75, 2) },
            new object[] { 4, 150, 0.3955661f, 0.8721925f, Color.FromArgb(4, 210, 235, 222) },
            new object[] { 140, 104, 0.3838348f, 0.8152012f, Color.FromArgb(140, 199, 226, 190) },
            new object[] { 190, 174, 0.8480943f, 0.3469547f, Color.FromArgb(190, 13, 164, 149) },
            new object[] { 105, 274, 0.4693375f, 0.68899f, Color.FromArgb(105, 181, 138, 213) },
            new object[] { 163, 50, 0.3324784f, 0.8536682f, Color.FromArgb(163, 230, 226, 205) },
            new object[] { 20, 180, 0.6970847f, 0.7858986f, Color.FromArgb(20, 162, 238, 238) },
            new object[] { 65, 313, 0.8841124f, 0.3663413f, Color.FromArgb(65, 176, 11, 140) },
            new object[] { 56, 328, 0.9566581f, 0.8109934f, Color.FromArgb(56, 253, 161, 210) },
            new object[] { 253, 35, 0.4184979f, 0.8820199f, Color.FromArgb(253, 238, 227, 212) },
            new object[] { 1, 248, 0.5103415f, 0.1174333f, Color.FromArgb(1, 19, 15, 45) },
            new object[] { 74, 108, 0.1981762f, 0.1279557f, Color.FromArgb(74, 29, 39, 26) },
            new object[] { 85, 36, 0.6369577f, 0.8818553f, Color.FromArgb(85, 244, 229, 206) },
            new object[] { 50, 160, 0.2304416f, 0.08916786f, Color.FromArgb(50, 17, 28, 24) },
            new object[] { 212, 37, 0.07586023f, 0.5181446f, Color.FromArgb(212, 141, 134, 123) },
            new object[] { 184, 219, 0.6178428f, 0.67761f, Color.FromArgb(184, 122, 158, 224) },
            new object[] { 103, 6, 0.595657f, 0.8336312f, Color.FromArgb(103, 238, 192, 187) },
            new object[] { 135, 13, 0.3570622f, 0.3127086f, Color.FromArgb(135, 108, 64, 51) },
            new object[] { 88, 211, 0.6832753f, 0.9470552f, Color.FromArgb(88, 232, 241, 251) },
            new object[] { 161, 77, 0.5628753f, 0.7785361f, Color.FromArgb(161, 212, 230, 167) },
            new object[] { 77, 204, 0.4531581f, 0.1518091f, Color.FromArgb(77, 21, 42, 56) },
            new object[] { 59, 0, 0.1678197f, 0.7742334f, Color.FromArgb(59, 207, 188, 188) },
            new object[] { 174, 312, 0.9804731f, 0.8851182f, Color.FromArgb(174, 254, 197, 243) },
            new object[] { 6, 324, 0.1614383f, 0.3960472f, Color.FromArgb(6, 117, 85, 104) },
            new object[] { 18, 299, 0.7119258f, 0.2732994f, Color.FromArgb(18, 118, 20, 119) },
            new object[] { 180, 131, 0.5995331f, 0.2956396f, Color.FromArgb(180, 30, 121, 47) },
            new object[] { 190, 116, 0.5365929f, 0.611204f, Color.FromArgb(190, 110, 209, 103) },
            new object[] { 195, 102, 0.4585954f, 0.6154752f, Color.FromArgb(195, 139, 202, 112) },
            new object[] { 138, 107, 0.2157484f, 0.1514526f, Color.FromArgb(138, 34, 47, 30) },
            // force going through the saturation == 0 branch
            new object[] { 127, 180, 0f, 0.5f, Color.FromArgb(127, 128, 128, 128) }
        };

        [Test, TestCaseSource("FromAhsbTestCases")]
        public static void TestFromAhsbWithVariousParametersExpectNoRegressions(int a, int h, float s, float b, Color expectedResult)
        {

#if GENERATE_MORE_TEST_CASES
            var r = new Random();
            for (int i = 0; i < 32; i++)
            {
                int alpha = r.Next(256);
                int hue = r.Next(360);
                float saturation = Convert.ToSingle(r.NextDouble());
                float brightness = Convert.ToSingle(r.NextDouble());

                Color c = ColorExtensions.FromAhsb(alpha, hue, saturation, brightness);

                Console.WriteLine(
                    "new object[] {{ {0}, {1}, {2}f, {3}f, Color.FromArgb({4}, {5}, {6}, {7}) }},",
                    alpha, hue, saturation, brightness, c.A, c.R, c.G, c.B);
            }
#endif

            Color result = ColorExtensions.FromAhsb(a, h, s, b);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
