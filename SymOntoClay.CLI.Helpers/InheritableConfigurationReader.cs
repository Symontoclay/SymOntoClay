using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.CoreHelper.DebugHelpers;
using Newtonsoft.Json;

namespace SymOntoClay.CLI.Helpers
{
    public static class InheritableConfigurationReader
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static T Read<T>(string fileName, string defaultConfigurationFileName)
            where T : class, IInheritableConfiguration
        {
#if DEBUG
            //_logger.Info($"fileName = {fileName}");
            //_logger.Info($"defaultConfigurationFileName = {defaultConfigurationFileName}");
#endif

            var defaultConfiguration = Read<T>(defaultConfigurationFileName);

            return Read<T>(fileName, defaultConfiguration);
        }

        public static T Read<T>(string fileName, T defaultConfiguration)
            where T : class, IInheritableConfiguration
        {
#if DEBUG
            //_logger.Info($"fileName = {fileName}");
            //_logger.Info($"defaultConfiguration = {defaultConfiguration}");
#endif

            var processedFileNames = new List<string>();

            return NRead<T>(fileName, ref processedFileNames, defaultConfiguration);
        }

        public static T Read<T>(string fileName)
            where T : class, IInheritableConfiguration
        {
#if DEBUG
            //_logger.Info($"fileName = {fileName}");
#endif

            var processedFileNames = new List<string>();

            return NRead<T>(fileName, ref processedFileNames, null);
        }

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        private static T NRead<T>(string fileName, ref List<string> processedFileNames, T? defaultConfiguration)
            where T : class, IInheritableConfiguration
        {
#if DEBUG
            //_logger.Info($"fileName = {fileName}");
            //_logger.Info($"processedFileNames = {processedFileNames.WritePODListToString()}");
#endif

            if(processedFileNames.Contains(fileName))
            {
                throw new Exception($"The file '{fileName}' has been used previously. A cyclic dependence happened.");
            }

            var cfg = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName), _jsonSerializerSettings);

#if DEBUG
            //_logger.Info($"cfg = {cfg}");
#endif

            if(string.IsNullOrWhiteSpace(cfg.ParentCfg))
            {
                if(defaultConfiguration != null)
                {
                    var childCgf = cfg;
                    cfg = defaultConfiguration;
                    cfg.Write(childCgf);
                }                
            }
            else
            {
                var childCgf = cfg;

                cfg = NRead<T>(childCgf.ParentCfg, ref processedFileNames, defaultConfiguration);

                cfg.Write(childCgf);
            }

#if DEBUG
            //_logger.Info($"cfg (2) = {cfg}");
#endif

            return cfg;
        }
    }
}
