using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public class ContextOfConverterFactToInternalCG
    {
        public InternalConceptualGraph ConceptualGraph { get; set; }
        public IEntityLogger Logger { get; set; }
        public INLPConverterContext NLPContext { get; set; }
        public BaseRulePart CurrentRulePart { get; set; }
        public Dictionary<string, string> VarConceptDict { get; set; } = new Dictionary<string, string>();
        public Dictionary<LogicalQueryNode, ResultOfNode> VisitedRelations { get; set; } = new Dictionary<LogicalQueryNode, ResultOfNode>();
    }
}
