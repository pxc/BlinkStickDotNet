using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Threading;
using HidLibrary;
using Timer = System.Timers.Timer;

namespace BlinkStickDotNet
{
    /// <summary>
    /// The quantity of output to generate
    /// </summary>
    public enum Verbosity
    {
        Silent = 0,
        Verbose,
        Debug
    }

    /// <summary>
    /// A model of the BlinkStick device itself
    /// </summary>
    public class BlinkStick : IDisposable
    {
        private const int VendorID = 0X20A0; // 8352
        private const int ProductID = 0x41E5; // 16969

        /// <summary>
        /// The BlinkStick hardware
        /// </summary>
        private readonly HidDevice m_stick;

        /// <summary>
        /// A timer to keep track of turning the BlinkStick off at the end of a blink
        /// </summary>
        private readonly Timer m_blinkEndTimer = new Timer();

        /// <summary>
        /// How verbose the output should be.
        /// </summary>
        public Verbosity Verbosity { get; set; }

        /// <summary>
        /// Some constants for controlling the brightness of the LED
        /// </summary>
        public enum Brightness
        {
            Dark,
            Darkish,
            Medium,
            Brightish,
            Bright
        }

        private readonly IReadOnlyDictionary<Brightness, float> m_brightnessValue = new Dictionary<Brightness, float> {
            { Brightness.Dark, 0.005f },
            { Brightness.Darkish, 0.05f },
            { Brightness.Medium, 0.1f },
            { Brightness.Brightish, 0.25f },
            { Brightness.Bright, 1f }
        };

        /// <summary>
        /// A maximum brighness to allow (0.0 to 1.0).
        /// </summary>
        /// <remarks>
        /// 0.0 and 1.0 both mean no limit.
        /// </remarks>
        public float BrightnessLimit { get; private set; }

        public void SetBrightnessLimit(Brightness brightness)
        {
            BrightnessLimit = m_brightnessValue[brightness];
        }

        private Color LimitBrightness(Color c)
        {
            var brighnessLimit = BrightnessLimit;

            if (brighnessLimit == 0f || brighnessLimit == 1f || c.GetBrightness() <= brighnessLimit)
            {
                return c;
            }

            return ColorExtensions.FromAhsb(c.A, c.GetHue(), c.GetSaturation(), brighnessLimit);
        }

        public Color LedColor
        {
            get
            {
                byte[] data;
                m_stick.ReadFeatureData(out data, 1);
                return Color.FromArgb(data[1], data[2], data[3]);
            }

            set
            {
                Color limitedValue = LimitBrightness(value);
                var data = new[] { (byte)1, limitedValue.R, limitedValue.G, limitedValue.B };
                m_stick.WriteFeatureData(data);
                WriteLine("LedColor set to {0}", limitedValue);
            }
        }

        public BlinkStick()
        {
            HidDevice[] devices = HidDevices.Enumerate(VendorID, ProductID).ToArray();

            if (!devices.Any())
            {
                throw new IOException("No BlinkStick found");
            }

            m_stick = devices.First();

            m_blinkEndTimer.Elapsed += OnBlinkEndTimerElapsed;
        }

        /// <summary>
        /// Start a task to turn the BlinkStick on for a specified time and then turn it off again.
        /// </summary>
        /// <param name="color">The colour</param>
        /// <param name="duration">The duration in milliseconds</param>
        public void Blink(Color color, int duration)
        {
            LedColor = color;
            SetBlinkEndTimer(duration);
        }

        /// <summary>
        /// Turn the BlinkStick on for a specified time and then turn it off again.
        /// </summary>
        /// <param name="color">The colour</param>
        /// <param name="duration">The duration in milliseconds</param>
        /// <remarks>This version pauses the thread of execution for the duration of the blink.</remarks>
        public void BlinkWait(Color color, int duration)
        {
            LedColor = color;
            Thread.Sleep(duration);
            TurnOff();
        }

        public void DoubleBlink(Color color, int flashTime, int gapTime)
        {
            BlinkWait(color, flashTime);
            Thread.Sleep(gapTime);
            Blink(color, flashTime);
        }

        public void Gradient(Color start, Color end, int steps, int timeBetweenSteps)
        {
            double redPerStep = (double)(end.R - start.R) / steps;
            double greenPerStep = (double)(end.G - start.G) / steps;
            double bluePerStep = (double)(end.B - start.B) / steps;

            for (int i = 0; i < steps; i++)
            {
                Color c = Color.FromArgb(
                    (int)(start.R + i * redPerStep),
                    (int)(start.G + i * greenPerStep),
                    (int)(start.B + i * bluePerStep)
                );

                LedColor = c;

                Thread.Sleep(timeBetweenSteps);
            }

            TurnOff();
        }

        public void TurnOff()
        {
            WriteDebugLine("Resetting to black");

            m_blinkEndTimer.Stop();

            LedColor = Color.Black;
        }

        /// <summary>
        /// Writes output to the console if the <see cref="Verbosity"/> allows output.
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="values">An optional set of values to format</param>
        internal void WriteLine(string format, params object[] values)
        {
            if (Verbosity != Verbosity.Silent)
            {
                Console.WriteLine(format, values);
            }
        }

        /// <summary>
        /// Writes output to the console if the <see cref="Verbosity"/> is set to allow debug output.
        /// </summary>
        /// <param name="format">A format string</param>
        /// <param name="values">An optional set of values to format</param>
        internal void WriteDebugLine(string format, params object[] values)
        {
            if (Verbosity == Verbosity.Debug || Debugger.IsAttached)
            {
                Console.WriteLine(format, values);                
            }
        }

        /// <summary>
        /// Helper method to turn off the LED after a specified delay
        /// </summary>
        /// <param name="delay">Delay in milliseconds</param>
        private void SetBlinkEndTimer(int delay)
        {
            m_blinkEndTimer.Stop();
            m_blinkEndTimer.Interval = delay;
            m_blinkEndTimer.AutoReset = false;
            m_blinkEndTimer.Start();
        }

        private void OnBlinkEndTimerElapsed(object sender, ElapsedEventArgs e)
        {
            WriteDebugLine("in OnBlinkEndTimerElapsed");
            TurnOff();
        }

        public void Dispose()
        {
            WriteDebugLine("in Dispose()");

            Dispose(true);
            GC.SuppressFinalize(this);

            WriteDebugLine("end of Dispose()");
        }

        private void Dispose(bool disposing)
        {
            WriteDebugLine("in Dispose({0})", disposing);

            if (disposing)
            {
                if (m_stick != null)
                {
                    while (m_blinkEndTimer.Enabled)
                    {
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(100); // make sure the event has time to fire
                    m_blinkEndTimer.Dispose();

                    TurnOff(); // turn off if we're full on (i.e. we weren't blinking)

                    WriteDebugLine("All work complete");

                    m_stick.Dispose();
                }                
            }
            else
            {
                if (m_stick != null)
                {
                    TurnOff();
                }                
            }

            WriteDebugLine("end of Dispose({0})", disposing);
        }

        ~BlinkStick()
        {
            WriteDebugLine("In ~BlinkStick()");

            Dispose(false);
        }
    }
}
