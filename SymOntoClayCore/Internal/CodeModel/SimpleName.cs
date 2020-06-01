using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SimpleName: IEquatable<SimpleName>, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public string DictionaryName { get; set; }

        public string NameValue { get; set; }
        public string FullNameValue { get; set; }

        public ulong FullNameKey { get; set; }

        public bool Equals(SimpleName other)
        {
            if (other == null)
            {
                return false;
            }

            return NameValue == other.NameValue && FullNameValue == other.FullNameValue && FullNameKey == other.FullNameKey;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var personObj = obj as SimpleName;
            if (personObj == null)
            {
                return false;
            }

            return Equals(personObj);
        }

        public override int GetHashCode()
        {
            return FullNameKey.GetHashCode();
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

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameValue)} = {FullNameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameKey)} = {FullNameKey}");
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

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameValue)} = {FullNameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameKey)} = {FullNameKey}");
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

            sb.AppendLine($"{spaces}{nameof(DictionaryName)} = {DictionaryName}");

            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameValue)} = {FullNameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameKey)} = {FullNameKey}");
            return sb.ToString();
        }
    }
}
