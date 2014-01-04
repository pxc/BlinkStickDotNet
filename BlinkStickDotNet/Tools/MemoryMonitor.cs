using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Microsoft.VisualBasic.Devices;

namespace BlinkStickDotNet.Tools
{
    /// <summary>
    /// Changes the LED color based on how much memory is being used
    /// </summary>
    public static class MemoryMonitor
    {
        /// <summary>
        /// Runs a memory monitor
        /// </summary>
        /// <param name="stick">The BlinkStick to use</param>
        /// <param name="keepGoing">A callback method; when this returns false, the loop stops</param>
        public static void Run(BlinkStick stick, Func<bool> keepGoing)
        {
            var bands = new SortedDictionary<float, Color>
            {
                { 20f, Color.Blue },
                { 50f, Color.Green },
                { 70f, Color.Yellow },
                { 90f, Color.Orange },
                { 100f, Color.Red }
            };

            ulong totalMemoryInBytes = new ComputerInfo().TotalPhysicalMemory;
            float totalMemoryInMegabytes = (float)((double)totalMemoryInBytes / (1024 * 1024));

            using (var pc = new PerformanceCounter("Memory", "Available Mbytes"))
            {
                while (keepGoing())
                {
                    float memoryUsagePercent = (100 * (totalMemoryInMegabytes - pc.NextValue())) / totalMemoryInMegabytes;

                    stick.WriteLine("memoryUsage = {0}", memoryUsagePercent);

                    stick.LedColor = ColorExtensions.ValueToColor(bands, memoryUsagePercent);

                    Thread.Sleep(1000);
                }
            }            
        }
    }
}
