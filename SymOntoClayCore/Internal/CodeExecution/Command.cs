/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class Command: ICommand
    {
        /// <inheritdoc/>
        public StrongIdentifierValue Name { get; set; }

        /// <inheritdoc/>
        public IList<Value> ParamsList { get; set; }

        /// <inheritdoc/>
        public IDictionary<StrongIdentifierValue, Value> ParamsDict { get; set; }

        /// <inheritdoc/>
        public int ParamsCount
        {
            get
            {
                if(ParamsList != null)
                {
                    return ParamsList.Count;
                }

                if(ParamsDict != null)
                {
                    return ParamsDict.Count;
                }

                return 0;
            }
        }

        /// <inheritdoc/>
        public KindOfCommandParameters KindOfCommandParameters
        {
            get
            {
                if (ParamsList != null)
                {
                    return KindOfCommandParameters.ParametersByList;
                }

                if (ParamsDict != null)
                {
                    return KindOfCommandParameters.ParametersByDict;
                }

                return KindOfCommandParameters.NoParameters;
            }
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

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintObjDict_1_Prop(n, nameof(ParamsDict), ParamsDict);

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

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintShortObjDict_1_Prop(n, nameof(ParamsDict), ParamsDict);

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

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(ParamsCount)} = {ParamsCount}");
            sb.AppendLine($"{spaces}{nameof(KindOfCommandParameters)} = {KindOfCommandParameters}");

            sb.PrintBriefObjListProp(n, nameof(ParamsList), ParamsList);

            sb.PrintBriefObjDict_1_Prop(n, nameof(ParamsDict), ParamsDict);

            return sb.ToString();
        }
    }
}
