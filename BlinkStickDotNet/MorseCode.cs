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
        public static IReadOnlyDictionary<MorseCodeElement, int> RelativeLengths = new Dictionary<MorseCodeElement, int>
        {
            { MorseCodeElement.Dot, 1 },
            { MorseCodeElement.Dash, 3 },
            { MorseCodeElement.InterElementGap, 1 },
            { MorseCodeElement.InterLetterGap, 3 },
            { MorseCodeElement.InterWordGap, 7 }
        };

        public static IReadOnlyDictionary<MorseCodeElement, bool> IsGap = new Dictionary<MorseCodeElement, bool>
        {
            { MorseCodeElement.Dot, false },
            { MorseCodeElement.Dash, false },
            { MorseCodeElement.InterElementGap, true },
            { MorseCodeElement.InterLetterGap, true },
            { MorseCodeElement.InterWordGap, true }
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

            return EncodeCharacters(message);
        }

        private static IEnumerable<MorseCodeElement> EncodeCharacters(string message)
        {
            var encodedMessage = new List<MorseCodeElement>();
            bool isAtStartOfWord = true;
            foreach (char c in message.ToUpper(CultureInfo.CurrentCulture))
            {
                isAtStartOfWord = EncodeCharacter(c, isAtStartOfWord, encodedMessage);
            }

            return encodedMessage;
        }

        private static bool EncodeCharacter(char c, bool isAtStartOfWord, List<MorseCodeElement> encodedMessage)
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
            return isAtStartOfWord;
        }

        static MorseCode()
        {
            BuildEncodingFromEncodingSimple();
        }

        /// <summary>
        /// Build the thing that's easy for computers to work with
        /// (<see cref="Encoding"/>) from the thing that's easy for
        /// humans to work with (<see cref="EncodingSimple"/>).
        /// </summary>
        private static void BuildEncodingFromEncodingSimple()
        {
            var d = new Dictionary<char, IEnumerable<MorseCodeElement>>();

            foreach (KeyValuePair<char, string> keyValuePair in EncodingSimple)
            {
                AddEncodingToDictionary(keyValuePair, d);
            }

            Encoding = d;
        }

        private static void AddEncodingToDictionary(KeyValuePair<char, string> keyValuePair, IDictionary<char, IEnumerable<MorseCodeElement>> d)
        {
            IEnumerable<MorseCodeElement> elements = GetMorseCodeElements(keyValuePair.Value.ToCharArray());
            d.Add(keyValuePair.Key, elements);
        }

        private static IEnumerable<MorseCodeElement> GetMorseCodeElements(IList<char> value)
        {
            var elements = new List<MorseCodeElement>();

            for (int i = 0; i < value.Count; i++)
            {
                AddElementsAtIndex(value[i], i, elements);
            }

            return elements;
        }

        private static void AddElementsAtIndex(char value, int i, ICollection<MorseCodeElement> elements)
        {
            if (i > 0)
            {
                elements.Add(MorseCodeElement.InterElementGap);
            }

            switch (value)
            {
                case '.':
                    elements.Add(MorseCodeElement.Dot);
                    break;

                case '-':
                    elements.Add(MorseCodeElement.Dash);
                    break;

                default:
                    string msg = string.Format("Unknown character {0}", value);
                    throw new NotSupportedException(msg);
            }
        }
    }
}
