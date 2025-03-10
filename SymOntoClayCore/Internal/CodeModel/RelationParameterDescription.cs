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

using SymOntoClay.Common;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RelationParameterDescription: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString
    {
        public StrongIdentifierValue Name { get; set; }
        public List<TypeInfo> TypesList { get; set; } = new List<TypeInfo>();
        public List<Value> MeaningRolesList { get; set; } = new List<Value>();

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public RelationParameterDescription Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public RelationParameterDescription Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RelationParameterDescription)context[this];
            }

            var result = new RelationParameterDescription();
            context[this] = result;

            result.Name = Name.Clone(context);
            result.TypesList = TypesList?.Select(p => p.Clone(context)).ToList();
            result.MeaningRolesList = MeaningRolesList?.Select(p => p.CloneValue(context)).ToList();

            return result;
        }

        public ulong GetLongConditionalHashCode(CheckDirtyOptions options)
        {
            Name.CheckDirty(options);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    item.CheckDirty(options);
                }
            }

            if (!MeaningRolesList.IsNullOrEmpty())
            {
                foreach (var item in MeaningRolesList)
                {
                    item.CheckDirty(options);
                }
            }


            var result = Name.GetLongConditionalHashCode(options);

            if (!TypesList.IsNullOrEmpty())
            {
                foreach (var item in TypesList)
                {
                    result ^= item.GetLongConditionalHashCode(options);
                }
            }

            if (!MeaningRolesList.IsNullOrEmpty())
            {
                foreach (var item in MeaningRolesList)
                {
                    result ^= item.GetLongConditionalHashCode(options);
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

            if (!MeaningRolesList.IsNullOrEmpty())
            {
                foreach (var item in MeaningRolesList)
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
            sb.PrintObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);


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
            sb.PrintShortObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);


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
            sb.PrintBriefObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder(Name.ToHumanizedString(options));

            if(TypesList.IsNullOrEmpty())
            {
                sb.Append(": any");
            }
            else
            {
                sb.Append(": (");
                sb.Append(string.Join("|", TypesList.Select(p => p.ToHumanizedString(options))));
                sb.Append(")");
            }

            if(!MeaningRolesList.IsNullOrEmpty())
            {
                sb.Append("[:");
                sb.Append(string.Join(",", MeaningRolesList.Select(p => p.ToHumanizedString(options))));
                sb.Append(":]");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedLabel(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}
