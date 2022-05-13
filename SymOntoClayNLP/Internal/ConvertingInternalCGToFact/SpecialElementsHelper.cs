using SymOntoClay.NLP.Internal.InternalCG;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingInternalCGToFact
{
    public static class SpecialElementsHelper
    {
        public static KindOfSpecialRelation GetKindOfSpecialRelation(string relationName)
        {
            if (string.IsNullOrWhiteSpace(relationName))
            {
                return KindOfSpecialRelation.Undefinded;
            }

            if (relationName == SpecialNamesOfRelations.ObjectRelationName)
            {
                return KindOfSpecialRelation.Object;
            }

            if (relationName == SpecialNamesOfRelations.ExperiencerRelationName)
            {
                return KindOfSpecialRelation.Experiencer;
            }

            if (relationName == SpecialNamesOfRelations.StateRelationName)
            {
                return KindOfSpecialRelation.State;
            }

            if (relationName == SpecialNamesOfRelations.AgentRelationName)
            {
                return KindOfSpecialRelation.Agent;
            }

            if (relationName == SpecialNamesOfRelations.ActionRelationName)
            {
                return KindOfSpecialRelation.Action;
            }

            if (relationName == SpecialNamesOfRelations.CommandRelationName)
            {
                return KindOfSpecialRelation.Command;
            }

            return KindOfSpecialRelation.Undefinded;
        }
    }
}
