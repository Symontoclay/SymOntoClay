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
    public class QueryExecutingCardAboutKnownInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public KindOfLogicalQueryNode Kind { get; set; }
        public ulong Key { get; set; }
        public Value Value { get; set; }
        public ulong? KeyOfVar { get; set; }
        public int? Position { get; set; }
        public BaseIndexedLogicalQueryNode Expression { get; set; }

        public QueryExecutingCardAboutKnownInfo Clone()
        {
            var result = new QueryExecutingCardAboutKnownInfo();
            result.Kind = Kind;
            result.Key = Key;
            result.Value = Value;
            result.Expression = Expression;
            return result;
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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");

            sb.PrintObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(KeyOfVar)} = {KeyOfVar}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintObjProp(n, nameof(Expression), Expression);

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");

            sb.PrintShortObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(KeyOfVar)} = {KeyOfVar}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintShortObjProp(n, nameof(Expression), Expression);

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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");

            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(KeyOfVar)} = {KeyOfVar}");
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);

            return sb.ToString();
        }
    }
}
