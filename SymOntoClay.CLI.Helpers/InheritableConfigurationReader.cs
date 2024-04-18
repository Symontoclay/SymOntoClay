/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

#if DEBUG
            //_logger.Info($"File.ReadAllText(fileName) = {File.ReadAllText(fileName)}");
#endif

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
