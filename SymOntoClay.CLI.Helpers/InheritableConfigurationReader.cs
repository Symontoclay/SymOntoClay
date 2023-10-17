﻿using NLog;
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static T Read<T>(string fileName)
            where T : class, IInheritableConfiguration
        {
#if DEBUG
            _logger.Info($"fileName = {fileName}");
#endif

            var processedFileNames = new List<string>();

            return NRead<T>(fileName, ref processedFileNames);
        }

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        private static T NRead<T>(string fileName, ref List<string> processedFileNames)
            where T : class, IInheritableConfiguration
        {
#if DEBUG
            _logger.Info($"fileName = {fileName}");
            _logger.Info($"processedFileNames = {processedFileNames.WritePODListToString()}");
#endif

            if(processedFileNames.Contains(fileName))
            {
                throw new Exception($"The file '{fileName}' has been used previously. A cyclic dependence happened.");
            }

            var cfg = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName), _jsonSerializerSettings);

#if DEBUG
            _logger.Info($"cfg = {cfg}");
#endif

            if(!string.IsNullOrWhiteSpace(cfg.ParentCfg))
            {
                var childCgf = cfg;

                cfg = NRead<T>(childCgf.ParentCfg, ref processedFileNames);

                cfg.Write(childCgf);
            }

#if DEBUG
            _logger.Info($"cfg (2) = {cfg}");
#endif

            return cfg;
        }
    }
}
