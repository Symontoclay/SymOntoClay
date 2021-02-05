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

using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class DebugHelperForLogicalSearchResult
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("en-GB");

        public static string ToString(LogicalSearchResult source, IEntityDictionary entityDictionary)
        {
            if (!source.IsSuccess)
            {
                return "<no>";
            }

            var sb = new StringBuilder();

            sb.AppendLine("<yes>");

            foreach (var item in source.Items)
            {
                var varItemsStrList = new List<string>();

                foreach (var resultOfVarOfQueryToRelation in item.ResultOfVarOfQueryToRelationList)
                {
                    var varName = entityDictionary.GetName(resultOfVarOfQueryToRelation.KeyOfVar);
                    var foundNode = resultOfVarOfQueryToRelation.FoundExpression;

                    varItemsStrList.Add($" {varName} = {DebugHelperForIndexedRuleInstance.ToString(foundNode, entityDictionary)}");
                }

                sb.AppendLine(string.Join(";", varItemsStrList).Trim());
            }

            return sb.ToString();
        }
    }
}
