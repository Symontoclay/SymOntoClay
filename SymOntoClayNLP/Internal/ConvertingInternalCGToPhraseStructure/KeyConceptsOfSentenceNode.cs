﻿using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public class KeyConceptsOfSentenceNode : IObjectToString
    {
        public BaseInternalConceptCGNode SubjectConcept { get; set; }
        public InternalConceptCGNode VerbConcept { get; set; }

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
            sb.PrintObjProp(n, nameof(VerbConcept), VerbConcept);

            return sb.ToString();
        }
    }
}
