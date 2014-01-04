using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlinkStickDotNet
{
    /// <summary>
    /// On and off elements of various lengths
    /// </summary>
    public enum MorseCodeElement
    {
        Dot,
        Dash,
        InterElementGap,
        InterLetterGap,
        InterWordGap
    }

    internal class MorseCode
    {
        public static IReadOnlyDictionary<MorseCodeElement, int> RelativeLengths = new Dictionary<MorseCodeElement, int>()
        {
            { MorseCodeElement.Dot, 1 },
            { MorseCodeElement.Dash, 3 },
            { MorseCodeElement.InterElementGap, 1 },
            { MorseCodeElement.InterLetterGap, 3 },
            { MorseCodeElement.InterWordGap, 7 }
        };

        /// <summary>
        /// A morse code encoding that's easy for humans to work with
        /// </summary>
        public static IReadOnlyDictionary<char, string> EncodingSimple = new Dictionary<char, string>
        {
            { 'A', ".-" },
            { 'B', "-..." },
            { 'C', "-.-." },
            { 'D', "-.." },
            { 'E', "." },
            { 'F', "..-." },
            { 'G', "--." },
            { 'H', "...." },
            { 'I', ".." },
            { 'J', ".---" },
            { 'K', "-.-" },
            { 'L', ".-.." },
            { 'M', "--" },
            { 'N', "-." },
            { 'O', "---" },
            { 'P', ".--." },
            { 'Q', "--.-" },
            { 'R', ".-." },
            { 'S', "..." },
            { 'T', "-" },
            { 'U', "..-" },
            { 'V', "...-" },
            { 'W', ".--" },
            { 'X', "-..-" },
            { 'Y', "-.--" },
            { 'Z', "--.." },
            { '1', ".----" },
            { '2', "..---" },
            { '3', "...--" },
            { '4', "....-" },
            { '5', "....." },
            { '6', "-...." },
            { '7', "--..." },
            { '8', "---.." },
            { '9', "----." },
            { '0', "-----" },
            { '.', ".-.-.-" },
            { ',', "--..--" },
            { '?', "..--.." },
            { '\'', ".----." },
            { '!', "-.-.--" },
            { '/', "-..-." },
            { '(', "-.--." },
            { ')', "-.--.-" },
            { '&', ".-..." },
            { ':', "---..." },
            { ';', "-.-.-." },
            { '=', "-...-" },
            { '+', ".-.-." },
            { '-', "-....-" },
            { '_', "..--.-" },
            { '"', ".-..-." },
            { '$', "...-..-" },
            { '@', ".--.-." },
            { 'ä', ".-.-" },
            { 'æ', ".-.-" },
            { 'ą', ".-.-" },
            { 'à', ".--.-" },
            { 'å', ".--.-" },
            { 'ç', "-.-.." },
            { 'ĉ', "-.-.." },
            { 'ć', "-.-.." },
            { 'š', "----" },
            { 'ð', "..--." },
            { 'ś', "...-..." },
            { 'è', ".-..-" },
            { 'ł', ".-..-" },
            { 'é', "..-.." },
            { 'đ', "..-.." },
            { 'ę', "..-.." },
            { 'ĝ', "--.-." },
            { 'ĥ', "----" },
            { 'ĵ', ".---." },
            { 'ź', "--..-." },
            { 'ñ', "--.--" },
            { 'ń', "--.--" },
            { 'ö', "---." },
            { 'ø', "---." },
            { 'ó', "---." },
            { 'ŝ', "...-." },
            { 'þ', ".--.." },
            { 'ü', "..--" },
            { 'ŭ', "..--" },
            { 'ż', "--..-" }
        };

        /// <summary>
        /// A morse code encoding that's easy for computers to work with
        /// </summary>
        public static IReadOnlyDictionary<char, IEnumerable<MorseCodeElement>> Encoding;

        /// <summary>
        /// Translate a message into a sequence of <see cref="MorseCodeElement"/>s
        /// </summary>
        /// <param name="message">The message to encode</param>
        /// <returns>A sequence of morse code elements</returns>
        public static IEnumerable<MorseCodeElement> Encode(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Enumerable.Empty<MorseCodeElement>();
            }

            var encodedMessage = new List<MorseCodeElement>();
            bool isAtStartOfWord = true;
            foreach (char c in message.ToUpper(CultureInfo.CurrentCulture))
            {
                if (Encoding.ContainsKey(c))
                {
                    if (!isAtStartOfWord)
                    {
                        encodedMessage.Add(MorseCodeElement.InterLetterGap);
                    }
                    encodedMessage.AddRange(Encoding[c]);
                    isAtStartOfWord = false;
                }
                else if (c == ' ')
                {
                    encodedMessage.Add(MorseCodeElement.InterWordGap);
                    isAtStartOfWord = true;
                }
            }

            return encodedMessage;
        }

        static MorseCode()
        {
            BuildEncodingFromEncodingSimple();
        }

        private static void BuildEncodingFromEncodingSimple()
        {
            var d = new Dictionary<char, IEnumerable<MorseCodeElement>>();

            foreach (KeyValuePair<char, string> keyValuePair in EncodingSimple)
            {
                var elements = new List<MorseCodeElement>();

                char[] value = keyValuePair.Value.ToCharArray();

                for (int i = 0; i < value.Length; i++)
                {
                    if (i > 0)
                    {
                        elements.Add(MorseCodeElement.InterElementGap);
                    }

                    switch (value[i])
                    {
                        case '.':
                            elements.Add(MorseCodeElement.Dot);
                            break;

                        case '-':
                            elements.Add(MorseCodeElement.Dash);
                            break;

                        default:
                            string msg = string.Format("Unknown character {0}", value[i]);
                            throw new NotSupportedException(msg);
                    }
                }

                d.Add(keyValuePair.Key, elements);
            }

            Encoding = d;
        }
    }
}
