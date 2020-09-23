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
            if(source.Items.IsNullOrEmpty())
            {
                return "<nothing>";
            }

            var sb = new StringBuilder();

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
