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
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CheckDirtyOptions: IObjectToString
    {
        public IEngineContext EngineContext { get; set; }
        public ILocalCodeExecutionContext LocalContext { get; set; }
        public bool ConvertWaypointValueFromSource { get; set; }
        public Dictionary<StrongIdentifierValue, StrongIdentifierValue> ReplaceConcepts { get; set; }
        public IList<StrongIdentifierValue> DontConvertConceptsToInhRelations { get; set; }
        public bool IgnoreStandaloneConceptsInNormalization { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CheckDirtyOptions Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CheckDirtyOptions Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CheckDirtyOptions)context[this];
            }

            var result = new CheckDirtyOptions();
            context[this] = result;

            result.EngineContext = EngineContext;
            result.LocalContext = LocalContext;
            result.ConvertWaypointValueFromSource = ConvertWaypointValueFromSource;
            result.ReplaceConcepts = ReplaceConcepts?.ToDictionary(p => p.Key, p => p.Value);
            result.DontConvertConceptsToInhRelations = DontConvertConceptsToInhRelations?.ToList();
            result.IgnoreStandaloneConceptsInNormalization = IgnoreStandaloneConceptsInNormalization;

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

            sb.PrintExisting(n, nameof(EngineContext), EngineContext);
            sb.PrintExisting(n, nameof(LocalContext), LocalContext);
            sb.AppendLine($"{spaces}{nameof(ConvertWaypointValueFromSource)} = {ConvertWaypointValueFromSource}");
            sb.PrintObjDict_1_Prop(n, nameof(ReplaceConcepts), ReplaceConcepts);
            sb.PrintObjListProp(n, nameof(DontConvertConceptsToInhRelations), DontConvertConceptsToInhRelations);
            sb.AppendLine($"{spaces}{nameof(IgnoreStandaloneConceptsInNormalization)} = {IgnoreStandaloneConceptsInNormalization}");

            return sb.ToString();
        }
    }
}
