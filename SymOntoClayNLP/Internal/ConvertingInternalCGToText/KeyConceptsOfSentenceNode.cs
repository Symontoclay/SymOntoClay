using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToText
{
    public class KeyConceptsOfSentenceNode : IObjectToString
    {
        public BaseInternalConceptCGNode SubjectConcept { get; set; }
        public List<string> DisabledSubjectRelations { get; set; }
        public InternalConceptCGNode VerbConcept { get; set; }
        public List<string> DisabledVerbRelations { get; set; }

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

            sb.PrintObjProp(n, nameof(SubjectConcept), SubjectConcept);
            sb.PrintPODList(n, nameof(DisabledSubjectRelations), DisabledSubjectRelations);
            sb.PrintObjProp(n, nameof(VerbConcept), VerbConcept);
            sb.PrintPODList(n, nameof(DisabledVerbRelations), DisabledVerbRelations);

            return sb.ToString();
        }
    }
}
