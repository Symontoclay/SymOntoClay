/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class ResultOfVarOfQueryToRelation : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public ulong KeyOfVar { get; set; }
        public BaseIndexedLogicalQueryNode FoundExpression { get; set; }
        public IDictionary<ulong, OriginOfVarOfQueryToRelation> OriginDict { get; set; } = new Dictionary<ulong, OriginOfVarOfQueryToRelation>();

        public ulong GetLongHashCode()
        {
            return (ulong)Math.Abs(KeyOfVar.GetHashCode()) ^ FoundExpression.GetLongHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KeyOfVar)} = {KeyOfVar}");
            sb.PrintObjProp(n, nameof(FoundExpression), FoundExpression);
            sb.PrintObjDict_2_Prop(n, nameof(OriginDict), OriginDict);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KeyOfVar)} = {KeyOfVar}");
            sb.PrintShortObjProp(n, nameof(FoundExpression), FoundExpression);
            sb.PrintShortObjDict_2_Prop(n, nameof(OriginDict), OriginDict);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KeyOfVar)} = {KeyOfVar}");
            sb.PrintBriefObjProp(n, nameof(FoundExpression), FoundExpression);
            sb.PrintBriefObjDict_2_Prop(n, nameof(OriginDict), OriginDict);

            return sb.ToString();
        }
    }
}
