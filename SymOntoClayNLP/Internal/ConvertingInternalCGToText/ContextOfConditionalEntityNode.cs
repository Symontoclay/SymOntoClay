using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToText
{
    public class ContextOfConditionalEntityNode
    {
        public string RootWord { get; set; } = string.Empty;
        public InternalConceptCGNode RootConcept { get; set; }
        public List<InternalRelationCGNode> VisitedRelations { get; set; } = new List<InternalRelationCGNode>();
    }
}
