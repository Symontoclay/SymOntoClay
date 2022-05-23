using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToPhraseStructure
{
    public abstract class BaseContextOfConvertingInternalCGToPhraseStructure
    {
        public IEntityLogger Logger { get; set; }
        public INLPConverterContext NLPContext { get; set; }
        public IWordsDict WordsDict { get; set; }
        public List<InternalRelationCGNode> VisitedRelations { get; set; } = new List<InternalRelationCGNode>();
    }
}
