using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class StorageUsingOptions : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public IStorage Storage { get; set; }
        public bool UseFacts { get; set; }
        public bool UseAdditionalInstances { get; set; }
        public bool UseProductions { get; set; }
        public int? MaxDeph { get; set; }
        public int Priority { get; set; }

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

            sb.PrintExisting(n, nameof(Storage), Storage);
            sb.AppendLine($"{spaces}{nameof(UseFacts)} = {UseFacts}");
            sb.AppendLine($"{spaces}{nameof(UseAdditionalInstances)} = {UseAdditionalInstances}");
            sb.AppendLine($"{spaces}{nameof(UseProductions)} = {UseProductions}");
            sb.AppendLine($"{spaces}{nameof(MaxDeph)} = {MaxDeph}");
            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");

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

            sb.PrintExisting(n, nameof(Storage), Storage);
            sb.AppendLine($"{spaces}{nameof(UseFacts)} = {UseFacts}");
            sb.AppendLine($"{spaces}{nameof(UseAdditionalInstances)} = {UseAdditionalInstances}");
            sb.AppendLine($"{spaces}{nameof(UseProductions)} = {UseProductions}");
            sb.AppendLine($"{spaces}{nameof(MaxDeph)} = {MaxDeph}");
            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");

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

            sb.PrintExisting(n, nameof(Storage), Storage);
            sb.AppendLine($"{spaces}{nameof(UseFacts)} = {UseFacts}");
            sb.AppendLine($"{spaces}{nameof(UseAdditionalInstances)} = {UseAdditionalInstances}");
            sb.AppendLine($"{spaces}{nameof(UseProductions)} = {UseProductions}");
            sb.AppendLine($"{spaces}{nameof(MaxDeph)} = {MaxDeph}");
            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");

            return sb.ToString();
        }
    }
}
