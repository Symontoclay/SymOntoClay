using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Helpers
{
    public static class PlatformTypesConverterHelper
    {
        public static EntityValue GetResolvedEntityValue(StrongIdentifierValue identifier, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            var entityValue = new EntityValue(identifier, context, localContext);

            entityValue.Resolve();

            return entityValue;
        }

        public static ConditionalEntityValue GetResolvedConditionalEntityValue(StrongIdentifierValue concept, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            var entityConditionExpression = new EntityConditionExpressionNode() { Kind = KindOfLogicalQueryNode.Concept };
            entityConditionExpression.Name = concept;

            var conditionalEntitySourceValue = new ConditionalEntitySourceValue(entityConditionExpression);
            conditionalEntitySourceValue.CheckDirty();

            var conditionalEntityValue = conditionalEntitySourceValue.ConvertToConditionalEntityValue(context, localContext);
            conditionalEntityValue.Resolve();

            return conditionalEntityValue;
        }
    }
}
