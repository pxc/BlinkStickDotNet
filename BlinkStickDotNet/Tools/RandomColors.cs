using System;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Tools
{
    /// <summary>
    /// Changes the LED color randomly
    /// </summary>
    public static class RandomColors
    {
        /// <summary>
        /// Runs a utility to change the LED to random colors at random durations
        /// </summary>
        /// <param name="stick">The BlinkStick to use</param>
        /// <param name="keepGoing">A callback method; when this returns false, the loop stops</param>
        public static void Run(BlinkStick stick, Func<bool> keepGoing)
        {
            var r = new Random();

            while (keepGoing())
            {
                Color c = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
                int duration = r.Next(100, 2000);

                stick.LedColor = c;

                Thread.Sleep(duration);
            }            
        }
    }
}
