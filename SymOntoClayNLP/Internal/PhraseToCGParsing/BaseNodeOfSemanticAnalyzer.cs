using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.Internal.CG;
using SymOntoClay.NLP.Internal.PhraseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseToCGParsing
{
    public abstract class BaseNodeOfSemanticAnalyzer
    {
        protected BaseNodeOfSemanticAnalyzer(ContextOfSemanticAnalyzer context)
        {
            Context = context;
        }

        protected ContextOfSemanticAnalyzer Context { get; private set; }
        protected RolesStorageOfSemanticAnalyzer PrimaryRolesDict { get; private set; } = new RolesStorageOfSemanticAnalyzer();

        protected void MarkAsEntityCondition(RelationCGNode relation)
        {
            var relationName = CGGramamaticalNamesOfRelations.EntityCondition;
            var conceptualGraph = Context.ConceptualGraph;

            var secondaryRelation = new RelationCGNode();
            secondaryRelation.Parent = conceptualGraph;
            secondaryRelation.Name = relationName;

            secondaryRelation.AddOutputNode(relation);
        }
    }
}
