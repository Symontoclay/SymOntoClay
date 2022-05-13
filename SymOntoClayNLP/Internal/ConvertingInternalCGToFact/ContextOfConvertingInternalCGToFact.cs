using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToFact
{
    public class ContextOfConvertingInternalCGToFact
    {
        public Dictionary<InternalConceptualGraph, RuleInstance> RuleInstancesDict { get; set; } = new Dictionary<InternalConceptualGraph, RuleInstance>();
        public List<RuleInstance> AnnotationsList { get; set; } = new List<RuleInstance>();

        private Dictionary<BaseInternalCGNode, List<InternalRelationCGNode>> mAnnotationRelations = new Dictionary<BaseInternalCGNode, List<InternalRelationCGNode>>();

        public void AddRelationAsAnnotation(BaseInternalCGNode target, InternalRelationCGNode annotatingRelation)
        {
            if (mAnnotationRelations.ContainsKey(target))
            {
                var list = mAnnotationRelations[target];

                if (!list.Contains(annotatingRelation))
                {
                    list.Add(annotatingRelation);
                }
                return;
            }

            mAnnotationRelations[target] = new List<InternalRelationCGNode>() { annotatingRelation };
        }

        public Dictionary<InternalRelationCGNode, BaseInternalConceptCGNode> ModifiedRelationsDict = new Dictionary<InternalRelationCGNode, BaseInternalConceptCGNode>();

        public List<InternalRelationCGNode> GetAnnotationRelations(InternalRelationCGNode annotatedRelation)
        {
            if (!ModifiedRelationsDict.ContainsKey(annotatedRelation))
            {
                return new List<InternalRelationCGNode>();
            }

            var sourceModifiedNode = ModifiedRelationsDict[annotatedRelation];

            if (mAnnotationRelations.ContainsKey(sourceModifiedNode))
            {
                return mAnnotationRelations[sourceModifiedNode];
            }

            return new List<InternalRelationCGNode>();
        }
    }
}
