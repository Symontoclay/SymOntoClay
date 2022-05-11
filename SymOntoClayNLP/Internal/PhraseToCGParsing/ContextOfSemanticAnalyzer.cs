using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public class ContextOfSemanticAnalyzer
    {
        public ConceptualGraph OuterConceptualGraph { get; set; }
        public ConceptualGraph ConceptualGraph { get; set; }
        public IWordsDict WordsDict { get; set; }
        public IEntityLogger Logger { get; set; }
        public RelationStorageOfSemanticAnalyzer RelationStorage { get; private set; } = new RelationStorageOfSemanticAnalyzer();
    }
}
