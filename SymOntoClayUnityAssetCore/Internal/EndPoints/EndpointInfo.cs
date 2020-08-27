using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndpointInfo : IEndpointInfo
    {
        public EndpointInfo()
        {
        }

        public EndpointInfo(IEndpointInfo source)
        {
            Name = source.Name;
            NeedMainThread = source.NeedMainThread;
            Devices = new List<int>(source.Devices);
            Arguments = new List<IEndpointArgumentInfo>(source.Arguments);
            MethodInfo = source.MethodInfo;
            Object = source.Object;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public bool NeedMainThread { get; set; }

        /// <inheritdoc/>
        public List<int> Devices { get; set; }

        /// <inheritdoc/>
        IReadOnlyList<int> IEndpointInfo.Devices => Devices;

        /// <inheritdoc/>
        public List<IEndpointArgumentInfo> Arguments { get; set; }

        IReadOnlyList<IEndpointArgumentInfo> IEndpointInfo.Arguments => Arguments;

        /// <inheritdoc/>
        public MethodInfo MethodInfo { get; set; }

        /// <inheritdoc/>
        public object Object { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintObjListProp(n, nameof(Arguments), Arguments);

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
            sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintShortObjListProp(n, nameof(Arguments), Arguments);

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
            sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintBriefObjListProp(n, nameof(Arguments), Arguments);

            return sb.ToString();
        }
    }
}
