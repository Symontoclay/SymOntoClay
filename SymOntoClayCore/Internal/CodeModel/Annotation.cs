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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Annotation: ItemWithLongHashCodes, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public List<RuleInstance> Facts { get; set; } = new List<RuleInstance>();
        public List<Value> MeaningRolesList { get; set; } = new List<Value>();
        public Dictionary<StrongIdentifierValue, Value> SettingsDict { get; set; } = new Dictionary<StrongIdentifierValue, Value>();
        public Dictionary<KindOfAnnotationSystemEvent, AnnotationSystemEvent> AnnotationSystemEventsDict { get; set; } = new Dictionary<KindOfAnnotationSystemEvent, AnnotationSystemEvent>();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            ulong result = 0;

            if(!Facts.IsNullOrEmpty())
            {
                foreach(var fact in Facts)
                {
                    result ^= fact.GetLongHashCode();
                }
            }

            if(!MeaningRolesList.IsNullOrEmpty())
            {
                foreach(var item in MeaningRolesList)
                {
                    result ^= item.GetLongHashCode();
                }
            }

            if(!SettingsDict.IsNullOrEmpty())
            {
                foreach(var item in SettingsDict)
                {
                    result ^= item.Key.GetLongHashCode();
                    result ^= item.Value.GetLongHashCode();
                }
            }

            return result;
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public Annotation Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public Annotation Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Annotation)context[this];
            }
            
            var result = new Annotation();
            context[this] = result;

            result.Facts = Facts?.Select(p => p.Clone(context)).ToList();
            result.MeaningRolesList = MeaningRolesList?.Select(p => p.CloneValue(context)).ToList();
            result.SettingsDict = SettingsDict?.ToDictionary(p => p.Key.Clone(context), p => p.Value.CloneValue(context));
            result.AnnotationSystemEventsDict?.ToDictionary(p => p.Key, p => p.Value.Clone(context));

            return result;
        }

        public void DiscoverAllAnnotations(IList<Annotation> result)
        {
            throw new NotImplementedException();
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

            sb.PrintObjListProp(n, nameof(Facts), Facts);
            sb.PrintObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);
            sb.PrintObjDict_1_Prop(n, nameof(SettingsDict), SettingsDict);
            sb.PrintObjDict_2_Prop(n, nameof(AnnotationSystemEventsDict), AnnotationSystemEventsDict);

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

            sb.PrintShortObjListProp(n, nameof(Facts), Facts);
            sb.PrintShortObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);
            sb.PrintShortObjDict_1_Prop(n, nameof(SettingsDict), SettingsDict);
            sb.PrintShortObjDict_2_Prop(n, nameof(AnnotationSystemEventsDict), AnnotationSystemEventsDict);

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

            sb.PrintBriefObjListProp(n, nameof(Facts), Facts);
            sb.PrintBriefObjListProp(n, nameof(MeaningRolesList), MeaningRolesList);
            sb.PrintBriefObjDict_1_Prop(n, nameof(SettingsDict), SettingsDict);
            sb.PrintBriefObjDict_2_Prop(n, nameof(AnnotationSystemEventsDict), AnnotationSystemEventsDict);

            return sb.ToString();
        }
    }
}
