using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.PlatformImplementations
{
    public class CommonNLogLogger : IPlatformLogger
    {
        private static readonly CommonNLogLogger __instance = new CommonNLogLogger();
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static CommonNLogLogger Instance => __instance;

        private CommonNLogLogger()
        {
        }

        public void WriteLn(string message)
        {
            _logger.Info(message);
        }
    }
}
