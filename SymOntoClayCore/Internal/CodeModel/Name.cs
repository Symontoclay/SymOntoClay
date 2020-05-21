﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Name: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public string OriginalNameValue { get; set; }
        public ulong OriginalNameKey { get; set; }
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
            sb.AppendLine($"{spaces}{nameof(OriginalNameValue)} = {OriginalNameValue}");
            sb.AppendLine($"{spaces}{nameof(OriginalNameKey)} = {OriginalNameKey}");
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
            sb.AppendLine($"{spaces}{nameof(OriginalNameValue)} = {OriginalNameValue}");
            sb.AppendLine($"{spaces}{nameof(OriginalNameKey)} = {OriginalNameKey}");
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
            sb.AppendLine($"{spaces}{nameof(OriginalNameValue)} = {OriginalNameValue}");
            sb.AppendLine($"{spaces}{nameof(OriginalNameKey)} = {OriginalNameKey}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");
            return sb.ToString();
        }
    }
}
