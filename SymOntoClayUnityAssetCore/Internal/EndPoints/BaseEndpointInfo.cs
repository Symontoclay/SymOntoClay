/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using System.Xml.Linq;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class BaseEndpointInfo: IBaseEndpointInfo
    {
        public BaseEndpointInfo()
        {
        }

        public BaseEndpointInfo(IBaseEndpointInfo source)
        {
            OriginalName = source.OriginalName;
            Name = source.Name;
            NeedMainThread = source.NeedMainThread;
            Devices = new List<int>(source.Devices);
            Friends = new List<string>(source.Friends);
            Arguments = new List<IEndpointArgumentInfo>(source.Arguments);
        }

        /// <inheritdoc/>
        public KindOfEndpointInfo KindOfEndpoint
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return KindOfEndpointInfo.Empty;
                }

                if (Name == "*")
                {
                    return KindOfEndpointInfo.GenericCall;
                }

                return KindOfEndpointInfo.Usual;
            }
        }

        /// <inheritdoc/>
        public string OriginalName { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public bool NeedMainThread { get; set; }

        /// <inheritdoc/>
        public List<int> Devices { get; set; }

        /// <inheritdoc/>
        IReadOnlyList<int> IBaseEndpointInfo.Devices => Devices;

        public List<string> Friends { get; set; }

        /// <inheritdoc/>
        IReadOnlyList<string> IBaseEndpointInfo.Friends => Friends;

        /// <inheritdoc/>
        public List<IEndpointArgumentInfo> Arguments { get; set; }

        IReadOnlyList<IEndpointArgumentInfo> IBaseEndpointInfo.Arguments => Arguments;

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

            sb.AppendLine($"{spaces}{nameof(KindOfEndpoint)} = {KindOfEndpoint}"); 
            sb.AppendLine($"{spaces}{nameof(OriginalName)} = {OriginalName}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintPODList(n, nameof(Friends), Friends);
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

            sb.AppendLine($"{spaces}{nameof(KindOfEndpoint)} = {KindOfEndpoint}");
            sb.AppendLine($"{spaces}{nameof(OriginalName)} = {OriginalName}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintPODList(n, nameof(Friends), Friends);
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

            sb.AppendLine($"{spaces}{nameof(KindOfEndpoint)} = {KindOfEndpoint}");
            sb.AppendLine($"{spaces}{nameof(OriginalName)} = {OriginalName}");
            sb.AppendLine($"{spaces}{nameof(Name)} = {Name}");
            sb.AppendLine($"{spaces}{nameof(NeedMainThread)} = {NeedMainThread}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintPODList(n, nameof(Friends), Friends);
            sb.PrintBriefObjListProp(n, nameof(Arguments), Arguments);

            return sb.ToString();
        }
    }
}
