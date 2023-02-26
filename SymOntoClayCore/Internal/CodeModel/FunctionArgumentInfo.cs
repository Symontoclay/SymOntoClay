/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FunctionArgumentInfo : IFunctionArgument
    {
        public StrongIdentifierValue Name { get; set; }
        public List<StrongIdentifierValue> TypesList { get; set; } = new List<StrongIdentifierValue>();
        public bool HasDefaultValue { get; set; }
        public Value DefaultValue { get; set; }

        /// <inheritdoc/>
        IList<StrongIdentifierValue> IFunctionArgument.TypesList => TypesList;

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public FunctionArgumentInfo Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public FunctionArgumentInfo Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (FunctionArgumentInfo)context[this];
            }

            var result = new FunctionArgumentInfo();
            context[this] = result;

            result.Name = Name?.Clone(context);
            result.TypesList = TypesList?.Select(p => p.Clone(context)).ToList();
            result.HasDefaultValue = HasDefaultValue;
            result.DefaultValue = DefaultValue?.CloneValue(context);

            return result;
        }

        public ulong GetLongHashCode(CheckDirtyOptions options)
        {
            Name.CheckDirty(options);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach(var item in TypesList)
                {
                    item.CheckDirty(options);
                }
            }

            DefaultValue?.CheckDirty(options);

            var result = Name.GetLongHashCode(options);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    result ^= item.GetLongHashCode(options);
                }
            }

            return result;
        }

        public void DiscoverAllAnnotations(IList<Annotation> result)
        {
            Name?.DiscoverAllAnnotations(result);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    item.DiscoverAllAnnotations(result);
                }
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

            sb.PrintObjListProp(n, nameof(TypesList), TypesList);

            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.PrintObjProp(n, nameof(DefaultValue), DefaultValue);

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
            sb.PrintShortObjListProp(n, nameof(TypesList), TypesList);
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.PrintShortObjProp(n, nameof(DefaultValue), DefaultValue);

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
            sb.PrintBriefObjListProp(n, nameof(TypesList), TypesList);
            sb.AppendLine($"{spaces}{nameof(HasDefaultValue)} = {HasDefaultValue}");
            sb.PrintBriefObjProp(n, nameof(DefaultValue), DefaultValue);

            return sb.ToString();
        }

        public string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
