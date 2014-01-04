using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Tools
{
    /// <summary>
    /// Changes the LED color based on how busy the CPU is
    /// </summary>
    public static class CpuMonitor
    {
        /// <summary>
        /// Runs a CPU monitor
        /// </summary>
        /// <param name="stick">The BlinkStick to use</param>
        /// <param name="keepGoing">A callback method; when this returns false, the loop stops</param>
        public static void Run(BlinkStick stick, Func<bool> keepGoing)
        {
            var bands = new SortedDictionary<float, Color> {
                                                               {20f, Color.Blue},
                                                               {50f, Color.Green},
                                                               {70f, Color.Yellow},
                                                               {90f, Color.Orange},
                                                               {100f, Color.Red}
                                                           };

            using (var pc = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                while (keepGoing())
                {
                    float cpuUsagePercent = pc.NextValue();

                    stick.WriteLine("cpuUsage = {0}", cpuUsagePercent);

                    stick.LedColor = ColorExtensions.ValueToColor(bands, cpuUsagePercent);

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
