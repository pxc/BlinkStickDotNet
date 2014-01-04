using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace BlinkStickDotNet
{
    /// <summary>
    /// Helper classes for working with colors
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Gets a color corresponding to a value by looking it up in a set of color bands
        /// </summary>
        /// <typeparam name="T">The type of values</typeparam>
        /// <param name="bands">A sequence of bands in ascending order</param>
        /// <param name="value">The value to look up</param>
        /// <returns>The color corresponding to the value</returns>
        public static Color ValueToColor<T>(SortedDictionary<T, Color> bands, T value) where T : IComparable
        {
            KeyValuePair<T, Color> band = bands.FirstOrDefault(kvp => kvp.Key.CompareTo(value) >= 0);

            Debug.WriteLine("Returning {0} for {1}", band.Value, value);

            return band.Value;
        }

        /// <summary>
        /// Creates a Color from alpha, hue, saturation and brightness.
        /// </summary>
        /// <param name="alpha">The alpha channel value (0-255).</param>
        /// <param name="hue">The hue value (0-260).</param>
        /// <param name="saturation">The saturation value (0-1).</param>
        /// <param name="brightness">The brightness value (0-1).</param>
        /// <returns>A Color with the given values.</returns>
        /// <remarks>
        /// Based on http://stackoverflow.com/questions/4106363/converting-rgb-to-hsb-colors
        /// </remarks>
        public static Color FromAhsb(int alpha, float hue, float saturation, float brightness)
        {
            AssertArgumentIsInRange(alpha, 0, 255, "alpha");
            AssertArgumentIsInRange(hue, 0, 360, "hue");
            AssertArgumentIsInRange(saturation, 0, 1, "saturation");
            AssertArgumentIsInRange(brightness, 0, 1, "brightness");

            if (saturation == 0)
            {
                return Color.FromArgb(
                    alpha,
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255),
                    Convert.ToInt32(brightness * 255));
            }

            float fMax, fMid, fMin;

            if (0.5 < brightness)
            {
                fMax = brightness - (brightness * saturation) + saturation;
                fMin = brightness + (brightness * saturation) - saturation;
            }
            else
            {
                fMax = brightness + (brightness * saturation);
                fMin = brightness - (brightness * saturation);
            }

            int iSextant = (int)Math.Floor(hue / 60f);
            if (300f <= hue)
            {
                hue -= 360f;
            }

            hue /= 60f;
            hue -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = (hue * (fMax - fMin)) + fMin;
            }
            else
            {
                fMid = fMin - (hue * (fMax - fMin));
            }

            int iMax = Convert.ToInt32(fMax * 255);
            int iMid = Convert.ToInt32(fMid * 255);
            int iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(alpha, iMid, iMax, iMin);

                case 2:
                    return Color.FromArgb(alpha, iMin, iMax, iMid);

                case 3:
                    return Color.FromArgb(alpha, iMin, iMid, iMax);

                case 4:
                    return Color.FromArgb(alpha, iMid, iMin, iMax);

                case 5:
                    return Color.FromArgb(alpha, iMax, iMin, iMid);

                default:
                    return Color.FromArgb(alpha, iMax, iMid, iMin);
            }
        }

        private static void AssertArgumentIsInRange<T>(T value, T minimum, T maximum, string name) where T : IComparable
        {
            if (value.CompareTo(minimum) < 0 || value.CompareTo(maximum) > 0)
            {
                string msg = string.Format("Value must be within a range of {0} - {1}.", minimum, maximum);
                throw new ArgumentOutOfRangeException(name, value, msg);
            }
        }
    }
}
