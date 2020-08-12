﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public class PlatformEndpointArgumentInfo : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool HasDefaultValue { get; set; }
        public object DefaultValue { get; set; }
        //public bool NeedMainThread { get; set; }
        //public IList<int> Devices { get; set; }

        public ParameterInfo ParameterInfo { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(Type)} = {Type.FullName}");
            sb.AppendLine($"{spaces}{nameof()} = {}");
            sb.AppendLine($"{spaces}{nameof()} = {}");

            //sb.PrintValueTypesListProp(n, nameof(Devices), Devices);

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

            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(Type)} = {Type.FullName}");

            //sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            //sb.PrintValueTypesListProp(n, nameof(Devices), Devices);

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

            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(Type)} = {Type.FullName}");

            //sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            //sb.PrintValueTypesListProp(n, nameof(Devices), Devices);

            return sb.ToString();
        }
    }
}
