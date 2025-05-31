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
using System.Linq;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class NameHelper
    {
        private static IMonitorLogger _logger = MonitorLoggerNLogImpementation.Instance;

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

        public static string ShieldString(string source)
        {
            if(string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            if (source.StartsWith("@>"))
            {
                var nameSubStr = source.Substring(2);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(2, "`")}`";
            }

            if (source.StartsWith("@@"))
            {
                var nameSubStr = source.Substring(2);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(2, "`")}`";
            }

            if (source.StartsWith("@:"))
            {
                var nameSubStr = source.Substring(2);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(2, "`")}`";
            }

            if (source == "#@")
            {
                return source;
            }

            if(source == "##@")
            {
                return source;
            }

            if (source.StartsWith("##@"))
            {
                var nameSubStr = source.Substring(2);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(3, "`")}`";
            }

            if (source.StartsWith("#@"))
            {
                var nameSubStr = source.Substring(2);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(2, "`")}`";
            }

            if (source.StartsWith("##"))
            {
                var nameSubStr = source.Substring(2);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(2, "`")}`";
            }

            if (source.StartsWith("#^"))
            {
                var nameSubStr = source.Substring(2);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(2, "`")}`";
            }

            if (source.StartsWith("@"))
            {
                var nameSubStr = source.Substring(1);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(1, "`")}`";
            }

            if (source.StartsWith("#"))
            {
                var nameSubStr = source.Substring(1);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(1, "`")}`";
            }

            if (source == "$_")
            {
                return source;
            }

            if (source.StartsWith("$"))
            {
                var nameSubStr = source.Substring(1);

                if (nameSubStr.All(p => char.IsLetterOrDigit(p) || p == '_') || nameSubStr.Contains("`"))
                {
                    return source;
                }

                return $"{source.Insert(1, "`")}`";
            }

            if (source.All(p => char.IsLetterOrDigit(p) || p == '_') || source.Contains("`"))
            {
                return source;
            }

            return $"`{source}`";
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

        private static Dictionary<string, StrongIdentifierValue> _cache = new Dictionary<string, StrongIdentifierValue>();

        public static StrongIdentifierValue CreateName(string text, IMonitorLogger logger)
        {
#if DEBUG
            _logger.Info("C853CD9C-C4D9-454F-A512-7D4E36FEADB7", $"text = '{text}'");
#endif

            if(_cache.TryGetValue(text, out var cachedResult))
            {
                return cachedResult;
            }

            var parserContext = new InternalParserCoreContext(text, logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

#if DEBUG
            //_logger.Info("938300DA-D7DD-4C09-8640-20A940EF7D7B", $"result = {result}");
#endif

            _cache[text] = result;

            return result;
            /*
            //if (text != "__ctor")
            //{
            var newValue = parser.Result;
                //newValue.Capacity = null;
                //if(!newValue.NormalizedNameValue.Contains("-"))
                //{
                //    newValue.NormalizedNameValue = newValue.NormalizedNameValue.Replace("`", string.Empty).Trim();
                //}
                return newValue;
            //}

            if (string.IsNullOrWhiteSpace(text))
            {
                var oldValue = new StrongIdentifierValue() { IsEmpty = true };

#if DEBUG
                //_logger.Info("9C2A8D73-7EF1-4EDA-8124-536EC05F744A", $"oldValue = {oldValue}");
#endif

                return oldValue;
            }

            var name = new StrongIdentifierValue() { IsEmpty = false };

            text = text.ToLower().Trim();

            if (text.Contains("::") || text.Contains("("))
            {
                throw new NotSupportedException("Symbols `::`, `(` and `)` are not supported yet!");
            }

            name.KindOfName = KindOfName.CommonConcept;

            if(text.StartsWith("@>"))
            {
                name.KindOfName = KindOfName.Channel;
            }
            else if (text.StartsWith("@@"))
            {
                name.KindOfName = KindOfName.SystemVar;
            }
            else if (text.StartsWith("@:"))
            {
                name.KindOfName = KindOfName.Property;
            }
            else if (text == "#@")
            {
                name.KindOfName = KindOfName.AnonymousEntityCondition;
            }
            else if (text.StartsWith("#@"))
            {
                name.KindOfName = KindOfName.EntityCondition;
            }
            else if (text.StartsWith("##"))
            {
                name.KindOfName = KindOfName.Concept;
            }
            else if (text.StartsWith("#^"))
            {
                name.KindOfName = KindOfName.RuleOrFact;
            }
            else if (text.StartsWith("#|"))
            {
                name.KindOfName = KindOfName.LinguisticVar;
            }
            else if (text.StartsWith("@"))
            {
                name.KindOfName = KindOfName.Var;
            }
            else if (text.StartsWith("#"))
            {
                name.KindOfName = KindOfName.Entity;
            }
            else if (text.StartsWith("$"))
            {
                name.KindOfName = KindOfName.LogicalVar;
            }

            name.NameValue = ShieldString(text);

            name.NormalizedNameValue = NormalizeString(text);

#if DEBUG
            //_logger.Info("88132CDC-4B18-40B5-BF08-7C1B512E7F7D", $"name = {name}");
#endif

            return name;*/
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
    }
}
