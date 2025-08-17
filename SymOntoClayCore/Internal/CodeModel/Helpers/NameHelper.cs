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

using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class NameHelper
    {
        private static IMonitorLogger _logger = MonitorLoggerNLogImplementation.Instance;

        public static string GetNewEntityNameString()
        {
            return $"#{Guid.NewGuid():D}";
        }

        public static string GetNewRuleOrFactNameString()
        {
            return $"#^{Guid.NewGuid():D}";
        }

        public static string GetNewLogicalVarNameString()
        {
            return $"${Guid.NewGuid().ToString("D").Substring(0, 8)}";
        }

        private static Dictionary<string, string> _shieldStringCache = new Dictionary<string, string>();
        private static object _shieldStringCacheLock = new object();

        public static string ShieldString(string source)
        {
            if(string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            lock (_shieldStringCacheLock)
            {
                if (_shieldStringCache.TryGetValue(source, out var cachedResult))
                {
                    return cachedResult;
                }
            }

            var name = CreateName(source);

            var result = name.NameValue;

            lock (_shieldStringCacheLock)
            {
                _shieldStringCache[source] = result;
            }

            return result;
        }

        public static string UnShieldString(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            return source.Replace("`", string.Empty);
        }

        public static StrongIdentifierValue CreateRuleOrFactName()
        {
            return CreateRuleOrFactName(_logger);
        }

        public static StrongIdentifierValue CreateRuleOrFactName(IMonitorLogger logger)
        {
            var text = GetNewRuleOrFactNameString();
            return CreateName(text, logger);
        }

        public static StrongIdentifierValue CreateEntityName()
        {
            return CreateEntityName(_logger);
        }

        public static StrongIdentifierValue CreateEntityName(IMonitorLogger logger)
        {
            var text = GetNewEntityNameString();
            return CreateName(text, logger);
        }

        public static StrongIdentifierValue CreateLogicalVarName()
        {
            return CreateLogicalVarName(_logger);
        }

        public static StrongIdentifierValue CreateLogicalVarName(IMonitorLogger logger)
        {
            var text = GetNewLogicalVarNameString();
            return CreateName(text, logger);
        }

        public static StrongIdentifierValue CreateName(string text)
        {
            return CreateName(text, _logger);
        }

        private static Dictionary<string, StrongIdentifierValue> _createNameCache = new Dictionary<string, StrongIdentifierValue>();
        private static object _createNameCacheLock = new object();

        public static StrongIdentifierValue CreateName(string text, IMonitorLogger logger)
        {
#if DEBUG
            //_logger.Info("C853CD9C-C4D9-454F-A512-7D4E36FEADB7", $"text = '{text}'");
#endif

            if(string.IsNullOrWhiteSpace(text))
            {
                return StrongIdentifierValue.Empty;
            }

            lock(_createNameCacheLock)
            {
                if (_createNameCache.TryGetValue(text, out var cachedResult))
                {
                    return cachedResult;
                }
            }

            var parserContext = new InternalParserCoreContext(text, logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

#if DEBUG
            //_logger.Info("938300DA-D7DD-4C09-8640-20A940EF7D7B", $"result = {result}");
#endif

            lock (_createNameCacheLock)
            {
                _createNameCache[text] = result;
            }            

            return result;
        }

        public static string NormalizeString(string value)
        {
            return value.ToLower().Replace("`", string.Empty).Trim();
        }

        public static StrongIdentifierValue CreateAlternativeArgumentName(StrongIdentifierValue argumentName, IMonitorLogger logger)
        {
            var nameValue = argumentName.NameValue;

            if (nameValue.StartsWith("@"))
            {
                return CreateName(argumentName.NameValue.Replace("@", string.Empty), logger);
            }

            return CreateName($"@{nameValue}", logger);
        }

        public static string GetNormalizedNameWithoutPrefixes(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if(value.Contains("@@"))
            {
                value = value.Replace("@@", string.Empty);
            }
            else
            {
                if(value.Contains("@"))
                {
                    value = value.Replace("@", string.Empty);
                }
            }

            return value.Trim();
        }
    }
}
