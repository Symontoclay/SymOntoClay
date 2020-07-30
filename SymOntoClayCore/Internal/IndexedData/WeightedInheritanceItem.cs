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
            IsSelf = source.IsSelf;
            Distance = source.Distance;
            SuperName = source.SuperName;
            Rank = source.Rank;
            OriginalIndexedItem = source.OriginalIndexedItem;
        }

        public bool IsSelf { get; set; }
        public uint Distance { get; set; }
        public IndexedStrongIdentifierValue SuperName { get; set; }
        public float Rank { get; set; }
        public IndexedInheritanceItem OriginalIndexedItem { get; set; }

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
