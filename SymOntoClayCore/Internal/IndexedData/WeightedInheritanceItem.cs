/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class WeightedInheritanceItem: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public WeightedInheritanceItem()
        {
        }

        public WeightedInheritanceItem(WeightedInheritanceItem source)
        {
            if(source != null)
            {
                IsSelf = source.IsSelf;
                Distance = source.Distance;
                SuperName = source.SuperName;
                Rank = source.Rank;
                OriginalItem = source.OriginalItem;
            }
        }

        public bool IsSelf { get; set; }
        public uint Distance { get; set; }
        public StrongIdentifierValue SuperName { get; set; }
        public float Rank { get; set; }
        public InheritanceItem OriginalItem { get; set; }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSelf)} = {IsSelf}");
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.PrintObjProp(n, nameof(SuperName), SuperName);
           
            sb.AppendLine($"{spaces}{nameof(Rank)} = {Rank}");

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
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSelf)} = {IsSelf}");
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");

            sb.PrintShortObjProp(n, nameof(SuperName), SuperName);

            sb.AppendLine($"{spaces}{nameof(Rank)} = {Rank}");

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
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSelf)} = {IsSelf}");
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");

            sb.PrintBriefObjProp(n, nameof(SuperName), SuperName);

            sb.AppendLine($"{spaces}{nameof(Rank)} = {Rank}");

            return sb.ToString();
        }
    }
}
