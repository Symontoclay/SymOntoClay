using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingCGToInternal
{
    public class ContextOfConvertingCGToInternal
    {
        //public IEntityDictionary EntityDictionary { get; set; }
        public List<ConceptualGraph> WrappersList { get; set; } = new List<ConceptualGraph>();
        public Dictionary<ConceptualGraph, InternalConceptualGraph> ConceptualGraphsDict { get; set; } = new Dictionary<ConceptualGraph, InternalConceptualGraph>();
        public Dictionary<ConceptCGNode, InternalConceptCGNode> ConceptsDict { get; set; } = new Dictionary<ConceptCGNode, InternalConceptCGNode>();
        public Dictionary<RelationCGNode, InternalRelationCGNode> RelationsDict { get; set; } = new Dictionary<RelationCGNode, InternalRelationCGNode>();
        public Dictionary<BaseCGNode, InternalConceptualGraph> EntityConditionsDict { get; set; } = new Dictionary<BaseCGNode, InternalConceptualGraph>();
    }
}
