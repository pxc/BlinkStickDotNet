using System.Collections.Generic;
using System.Drawing;

namespace BlinkStickDotNet.Tools
{
    /// <summary>
    /// Flashes the LED to transmit a morse code message
    /// </summary>
    public static class MorseCodeFlasher
    {
        /// <summary>
        /// Runs a morse code flasher
        /// </summary>
        /// <param name="stick">The BlinkStick to use</param>
        /// <param name="message">The message to transmit</param>
        /// <param name="color">The color to flash</param>
        /// <param name="dotLength">The duration of a morse dot in milliseconds</param>
        public static void Run(BlinkStick stick, string message, Color color, int dotLength)
        {
            IEnumerable<MorseCodeElement> encodedMessage = MorseCode.Encode(message);

            foreach (MorseCodeElement morseCodeElement in encodedMessage)
            {
                Color colorToFlash =
                    morseCodeElement == MorseCodeElement.Dash || morseCodeElement == MorseCodeElement.Dot
                    ? color : Color.Black;

                int duration = MorseCode.RelativeLengths[morseCodeElement] * dotLength;

                stick.BlinkWait(colorToFlash, duration);
            }

            stick.TurnOff();            
        }
    }
}
