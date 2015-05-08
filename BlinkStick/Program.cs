using System;
using System.Drawing;
using BlinkStickDotNet;
using BlinkStickDotNet.Tools;

namespace BlinkStickApp
{
    /// <summary>
    /// A command-line utility for controlling a BlinkStick.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Used to terminate infinite loops gracefully when the user presses Ctrl+C
        /// </summary>
        private static bool s_userWantsToQuit;

        static void Main(string[] args)
        {
            var parsedArguments = new Arguments(args);

            using (var stick = new BlinkStick())
            {
                HandleCtrlC();
                ProcessVerbosity(parsedArguments, stick);
                ProcessDebug(parsedArguments, stick);
                ProcessBrightness(parsedArguments, stick);
                Color color = ProcessColor(parsedArguments);
                int? duration = ProcessDuration(parsedArguments);
                ProcessTools(parsedArguments, stick, color, duration);
            }
        }

        private static void ProcessTools(Arguments parsedArguments, BlinkStick stick, Color color, int? duration)
        {
            if (parsedArguments.Has("test"))
            {
                Test(stick);
            }
            else if (parsedArguments.Has("random"))
            {
                RandomColors.Run(stick, KeepGoing);
            }
            else if (parsedArguments.Has("blink"))
            {
                const int defaultBlinkDuration = 1000;
                Blink(stick, color, duration ?? defaultBlinkDuration);
            }
            else if (parsedArguments.Has("mouseover"))
            {
                MouseOverColorTracker.Run(stick, KeepGoing);
            }
            else if (parsedArguments.Has("cpu"))
            {
                CpuMonitor.Run(stick, KeepGoing);
            }
            else if (parsedArguments.Has("memory"))
            {
                MemoryMonitor.Run(stick, KeepGoing);
            }
            else if (parsedArguments.Has("morse"))
            {
                string message = parsedArguments["morse"];

                const int defaultDotLength = 200;

                MorseCodeFlasher.Run(stick, message, color, duration ?? defaultDotLength);
            }
            else
            {
                DisplayUsage();
            }
        }

        private static int? ProcessDuration(Arguments parsedArguments)
        {
            int? duration = null;
            if (parsedArguments.Has("duration"))
            {
                int parsedDuration;
                if (int.TryParse(parsedArguments["duration"], out parsedDuration))
                {
                    duration = parsedDuration;
                }
                else
                {
                    DisplayUsage();
                }
            }
            return duration;
        }

        private static Color ProcessColor(Arguments parsedArguments)
        {
            Color color;
            if (parsedArguments.Has("color"))
            {
                color = ColorTranslator.FromHtml(parsedArguments["color"]);
            }
            else
            {
                color = Color.White;
            }
            return color;
        }

        private static void ProcessBrightness(Arguments parsedArguments, BlinkStick stick)
        {
            if (parsedArguments.Has("brightness"))
            {
                string rawBrightness = parsedArguments["brightness"];

                BlinkStick.Brightness brightness;
                if (Enum.TryParse(rawBrightness, true, out brightness))
                {
                    stick.SetBrightnessLimit(brightness);
                }
            }
        }

        private static void ProcessDebug(Arguments parsedArguments, BlinkStick stick)
        {
            if (parsedArguments.Has("debug"))
            {
                stick.Verbosity = Verbosity.Debug;
            }
        }

        private static void ProcessVerbosity(Arguments parsedArguments, BlinkStick stick)
        {
            if (parsedArguments.Has("verbose"))
            {
                stick.Verbosity = Verbosity.Verbose;
            }
        }

        private static void HandleCtrlC()
        {
            Console.CancelKeyPress += (obj, eventArgs) =>
            {
                s_userWantsToQuit = true;

                // make execution continue after this delegate completes
                eventArgs.Cancel = true;
            };
        }

        private static void Test(BlinkStick stick)
        {
            stick.BlinkWait(Color.Red, 1000);
            stick.BlinkWait(Color.Blue, 1000);
            stick.BlinkWait(Color.Green, 1000);
        }

        private static void Blink(BlinkStick stick, Color color, int duration)
        {
            stick.Blink(color, duration);
        }

        private static bool KeepGoing()
        {
            return !s_userWantsToQuit;
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("BlinkStick application.");
            Console.WriteLine("Usage: BlinkStick [--verbose|--debug]");
            Console.WriteLine("                  [--blink|--test|--random|--mouseover|--cpu|--memory|--morse message|--help]");
            Console.WriteLine("                  [--brightness dark|darkish|medium|brightish|bright]");
            Console.WriteLine("                  [--color #RRGGBB]");
            Console.WriteLine("                  [--duration 123]");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("BlinkStick.exe --blink --duration 500   Blink for 500 milliseconds");
            Console.WriteLine("BlinkStick.exe --test                   Flash three times");
            Console.WriteLine("BlinkStick.exe --random                 Cycle through colors at random");
            Console.WriteLine("BlinkStick.exe --mouseover              Start a mouseover monitor");
            Console.WriteLine("BlinkStick.exe --cpu --brightness dark  Start a (muted) CPU monitor");
            Console.WriteLine("BlickStick.exe --memory                 Start a memory monitor");
            Console.WriteLine("BlinkStick.exe --morse SOS              Send an SOS in morse code");
            Console.WriteLine();
            Console.WriteLine("Use Ctrl+C to exit long-running tasks");
        }
    }
}
