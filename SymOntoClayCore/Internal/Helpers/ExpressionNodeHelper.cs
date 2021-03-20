/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ExpressionNodeHelper
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static bool Compare(LogicalQueryNode expressionNode1, LogicalQueryNode expressionNode2, List<StrongIdentifierValue> additionalKeys_1, List<StrongIdentifierValue> additionalKeys_2
#if DEBUG
            , IEntityLogger logger
#endif
            )
        {
#if DEBUG
            //logger.Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
            _gbcLogger.Info($"expressionNode1 = {expressionNode1}");
            _gbcLogger.Info($"expressionNode2 = {expressionNode2}");
            //_gbcLogger.Info($"additionalKeys_1 = {JsonConvert.SerializeObject(additionalKeys_1?.Select(p => p.NameValue), Formatting.Indented)}");
            //_gbcLogger.Info($"additionalKeys_2 = {JsonConvert.SerializeObject(additionalKeys_2?.Select(p => p.NameValue), Formatting.Indented)}");
#endif

            if (expressionNode1.IsKeyRef && expressionNode2.IsKeyRef)
            {
                var key_1 = expressionNode1.Name;
                var key_2 = expressionNode2.Name;

                if (key_1 == key_2)
                {
#if DEBUG
                    //_gbcLogger.Info($"key_1 == key_2 = {key_1 == key_2}");
#endif
                    return true;
                }

                if(additionalKeys_1 != null && additionalKeys_1.Any(p => p == key_2))
                {
                    return true;
                }

                if(additionalKeys_2 != null && additionalKeys_2.Any(p => p == key_1))
                {
                    return true;
                }

                if(additionalKeys_1 != null && additionalKeys_2 != null && additionalKeys_1.Intersect(additionalKeys_2).Any())
                {
                    return true;
                }

                return false;
            }

            if (expressionNode1.Kind == KindOfLogicalQueryNode.Value && expressionNode2.Kind == KindOfLogicalQueryNode.Value)
            {
#if DEBUG
                //_gbcLogger.Info($"(expressionNode1.AsValue == null) = {expressionNode1.AsValue == null} (expressionNode2.AsValue == null) = {expressionNode2.AsValue == null}");
                _gbcLogger.Info($"expressionNode1.Value.GetSystemValue() = {expressionNode1.Value.GetSystemValue()}");
                _gbcLogger.Info($"expressionNode2.Value.GetSystemValue() = {expressionNode2.Value.GetSystemValue()}");
#endif

                if (expressionNode1.Value.GetSystemValue().Equals(expressionNode2.Value.GetSystemValue()))
                {
                    return true;
                }

                return false;
            }

            throw new NotImplementedException();
        }
    }
}
