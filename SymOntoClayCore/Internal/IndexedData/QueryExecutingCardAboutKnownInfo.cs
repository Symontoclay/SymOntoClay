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
        public StrongIdentifierValue Name { get; set; }
        public Value Value { get; set; }
        public StrongIdentifierValue NameOfVar { get; set; }
        public int? Position { get; set; }
        public LogicalQueryNode Expression { get; set; }

        public QueryExecutingCardAboutKnownInfo Clone()
        {
            var result = new QueryExecutingCardAboutKnownInfo();
            result.Kind = Kind;
            result.Name = Name;
            result.Value = Value;
            result.NameOfVar = NameOfVar;
            result.Position = Position;
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
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjProp(n, nameof(Value), Value);

            sb.PrintObjProp(n, nameof(NameOfVar), NameOfVar);
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
           
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjProp(n, nameof(Value), Value);

            sb.PrintShortObjProp(n, nameof(NameOfVar), NameOfVar);
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
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.PrintBriefObjProp(n, nameof(NameOfVar), NameOfVar);
            sb.AppendLine($"{spaces}{nameof(Position)} = {Position}");

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);

            return sb.ToString();
        }
    }
}
