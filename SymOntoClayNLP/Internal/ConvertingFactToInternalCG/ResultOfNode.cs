using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using SymOntoClay.NLP.Internal.PhraseToCGParsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ResultOfNode : IObjectToString
    {
        public BaseInternalConceptCGNode ConceptCGNode { get; set; }
        public LogicalQueryNode LogicalQueryNode { get; set; }

        public override string ToString()
        {
            return ToString(0u);
        }

        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(ConceptCGNode), ConceptCGNode);
            sb.PrintObjProp(n, nameof(LogicalQueryNode), LogicalQueryNode);

            return sb.ToString();
        }
    }
}
