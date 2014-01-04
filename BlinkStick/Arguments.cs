using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlinkStickApp
{
    /// <summary>
    /// Parse a sequence of argument strings supplied to a console application
    /// </summary>
    /// <remarks>
    /// Adapted from http://www.codeproject.com/Articles/3111/C-NET-Command-Line-Arguments-Parser
    /// </remarks>
    public class Arguments
    {
        // Variables
        private readonly Dictionary<string, string> m_parameters = new Dictionary<string, string>();

        // Constructor
        public Arguments(IEnumerable<string> args)
        {
            var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string txt in args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                string[] parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!m_parameters.ContainsKey(parameter))
                            {
                                parts[0] = remover.Replace(parts[0], "$1");
                                m_parameters.Add(parameter, parts[0]);
                            }

                            parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!m_parameters.ContainsKey(parameter))
                            {
                                m_parameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!m_parameters.ContainsKey(parameter))
                            {
                                m_parameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!m_parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            m_parameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!m_parameters.ContainsKey(parameter))
                {
                    m_parameters.Add(parameter, "true");
                }
            }
        }

        /// <summary>
        /// Retrieve a parameter value if it exists
        /// </summary>
        /// <param name="param">Name of the parameter to retrieve</param>
        /// <returns>The value of the parameter, or <c>null</c> if there was no such parameter</returns>
        public string this[string param]
        {
            get
            {
                string value;
                m_parameters.TryGetValue(param, out value);
                return value;
            }
        }

        /// <summary>
        /// Helper method to determine whether or not a parameter has been set.
        /// </summary>
        /// <param name="param">The name of the parameter to look for</param>
        /// <returns>True if the parameter has been set; false otherwise</returns>
        public bool Has(string param)
        {
            return this[param] != null;
        }
    }
}
