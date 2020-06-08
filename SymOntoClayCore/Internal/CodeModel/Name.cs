using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Name: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public string DictionaryName { get; set; }

        public bool IsEmpty { get; set; } = true;

        public KindOfName Kind { get; set; } = KindOfName.Unknown;
        public string NameValue { get; set; }
        public ulong NameKey { get; set; }

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

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
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(IsEmpty)} = {IsEmpty}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            return sb.ToString();
        }
    }
}
