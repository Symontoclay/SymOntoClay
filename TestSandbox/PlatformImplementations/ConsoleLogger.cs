using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.PlatformImplementations
{
    public class ConsoleLogger: IPlatformLogger
    {
        private static readonly ConsoleLogger __instance = new ConsoleLogger();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static ConsoleLogger Instance => __instance;

        private ConsoleLogger()
        {
        }

        public void WriteLn(string message)
        {
            Console.WriteLine(message);
        }
    }
}
