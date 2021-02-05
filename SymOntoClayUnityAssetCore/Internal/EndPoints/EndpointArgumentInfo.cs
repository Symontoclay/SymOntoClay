/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
