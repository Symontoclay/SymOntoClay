/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
