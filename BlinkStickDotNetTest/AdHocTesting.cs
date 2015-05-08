using System;
using System.Drawing;
using System.Threading;
using BlinkStickDotNet;
using NUnit.Framework;

namespace BlinkStickDotNetTest
{
    class AdHocTesting
    {
        /// <summary>
        /// Helper for doing ad-hoc testing
        /// </summary>
        public static void Main()
        {
            // add code here
#if false
            var bs = new BlinkStick();
            bs.Verbosity = Verbosity.Debug;
            bs.Blink(Color.Red, 1000);
#endif
        }
    }
}
