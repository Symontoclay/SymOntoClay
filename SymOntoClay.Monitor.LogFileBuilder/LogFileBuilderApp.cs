using NLog;
using SymOntoClay.CLI.Helpers;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileBuilderApp
    {
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void Run(string[] args, string defaultConfigurationFileName)
        {
#if DEBUG
            _logger.Info($"args = {args.WritePODListToString()}");
            _logger.Info($"defaultConfigurationFileName = {defaultConfigurationFileName}");
#endif

            var defaultConfiguration = string.IsNullOrWhiteSpace(defaultConfigurationFileName) ? null : InheritableConfigurationReader.Read<LogFileCreatorInheritableOptions>(EVPath.Normalize(defaultConfigurationFileName));

            Run(args, defaultConfiguration);
        }

        public void Run(string[] args, LogFileCreatorInheritableOptions defaultConfiguration)
        {
#if DEBUG
            _logger.Info($"args = {args.WritePODListToString()}");
            _logger.Info($"defaultConfiguration = {defaultConfiguration}");
#endif

            throw new NotImplementedException();
        }
    }
}
