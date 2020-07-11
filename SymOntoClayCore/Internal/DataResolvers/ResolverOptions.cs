﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ResolverOptions : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public bool IsDeepMode { get; set; }
        public bool AddSelf { get; set; }
        public bool JustDistinct { get; set; }

        public ResolverOptions Clone()
        {
            var result = new ResolverOptions();
            result.IsDeepMode = IsDeepMode;
            result.AddSelf = AddSelf;
            result.JustDistinct = JustDistinct;

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

            sb.AppendLine($"{spaces}{nameof(IsDeepMode)} = {IsDeepMode}");
            sb.AppendLine($"{spaces}{nameof(AddSelf)} = {AddSelf}");
            sb.AppendLine($"{spaces}{nameof(JustDistinct)} = {JustDistinct}");

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

            sb.AppendLine($"{spaces}{nameof(IsDeepMode)} = {IsDeepMode}");
            sb.AppendLine($"{spaces}{nameof(AddSelf)} = {AddSelf}");
            sb.AppendLine($"{spaces}{nameof(JustDistinct)} = {JustDistinct}");

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

            sb.AppendLine($"{spaces}{nameof(IsDeepMode)} = {IsDeepMode}");
            sb.AppendLine($"{spaces}{nameof(AddSelf)} = {AddSelf}");
            sb.AppendLine($"{spaces}{nameof(JustDistinct)} = {JustDistinct}");

            return sb.ToString();
        }

        public static ResolverOptions GetDefaultDeepOptions()
        {
            return new ResolverOptions() { IsDeepMode = true };
        }

        public static ResolverOptions GetDefaultFluentOptions()
        {
            return new ResolverOptions() { IsDeepMode = false };
        }
    }
}
