using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class EndpointArgumentInfo : IEndpointArgumentInfo
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public Type Type { get; set; }

        /// <inheritdoc/>
        public bool HasDefaultValue { get; set; }

        /// <inheritdoc/>
        public object DefaultValue { get; set; }

        /// <inheritdoc/>
        public int PositionNumber { get; set; }

        /// <inheritdoc/>
        public KindOfEndpointParam KindOfParameter { get; set; }

        /// <inheritdoc/>
        public bool IsSystemDefiend { get; set; }

        /// <inheritdoc/>
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
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.AppendLine($"{spaces}{nameof(DefaultValue)} = {DefaultValue}");

            sb.AppendLine($"{spaces}{nameof(PositionNumber)} = {PositionNumber}");
            sb.AppendLine($"{spaces}{nameof(KindOfParameter)} = {KindOfParameter}");

            sb.AppendLine($"{spaces}{nameof(IsSystemDefiend)} = {IsSystemDefiend}");

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
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.AppendLine($"{spaces}{nameof(DefaultValue)} = {DefaultValue}");

            sb.AppendLine($"{spaces}{nameof(PositionNumber)} = {PositionNumber}");
            sb.AppendLine($"{spaces}{nameof(KindOfParameter)} = {KindOfParameter}");

            sb.AppendLine($"{spaces}{nameof(IsSystemDefiend)} = {IsSystemDefiend}");

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
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.AppendLine($"{spaces}{nameof(DefaultValue)} = {DefaultValue}");

            sb.AppendLine($"{spaces}{nameof(PositionNumber)} = {PositionNumber}");
            sb.AppendLine($"{spaces}{nameof(KindOfParameter)} = {KindOfParameter}");

            sb.AppendLine($"{spaces}{nameof(IsSystemDefiend)} = {IsSystemDefiend}");

            return sb.ToString();
        }
    }
}
