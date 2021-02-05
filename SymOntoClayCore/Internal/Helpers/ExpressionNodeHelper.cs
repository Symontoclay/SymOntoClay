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
        public static bool Compare(BaseIndexedLogicalQueryNode expressionNode1, BaseIndexedLogicalQueryNode expressionNode2, List<ulong> additionalKeys
#if DEBUG
            , IEntityLogger logger
#endif
            )
        {
#if DEBUG
            //logger.Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
            //logger.Log($"expressionNode1 = {expressionNode1}");
            //logger.Log($"expressionNode2 = {expressionNode2}");
            //logger.Log($"additionalKeys = {JsonConvert.SerializeObject(additionalKeys, Formatting.Indented)}");
#endif

            if (expressionNode1.IsKeyRef && expressionNode2.IsKeyRef)
            {
                var key = expressionNode2.AsKeyRef.Key;

                if (expressionNode1.AsKeyRef.Key == key)
                {
                    return true;
                }

                if(additionalKeys != null && additionalKeys.Any(p => p == key))
                {
                    return true;
                }

                return false;
            }

            throw new NotImplementedException();
        }
    }
}
