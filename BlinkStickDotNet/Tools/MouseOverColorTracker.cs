using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace BlinkStickDotNet.Tools
{
    /// <summary>
    /// Changes the LED color based on the color under the mouse pointer
    /// </summary>
    public static class MouseOverColorTracker
    {
        /// <summary>
        /// Runs a mouse-over color tracker
        /// </summary>
        /// <param name="stick">The BlinkStick to use</param>
        /// <param name="keepGoing">A callback method; when this returns false, the loop stops</param>
        public static void Run(BlinkStick stick, Func<bool> keepGoing)
        {
            if (stick == null)
            {
                throw new ArgumentNullException("stick");
            }

            while (keepGoing())
            {
                Point pos = Cursor.Position;
                Color c = GetColorAt(pos);
                stick.LedColor = c;
                stick.WriteLine("Color at {0} is {1}", pos, c);
                Thread.Sleep(200);
            }
        }

        private static Color GetColorAt(Point location)
        {
            using (var screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(screenPixel))
                {
                    g.CopyFromScreen(location.X, location.Y, 0, 0, new Size(1, 1));
                }

                return screenPixel.GetPixel(0, 0);
            }
        }
    }
}
