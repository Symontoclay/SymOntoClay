using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToCG
{
    public static class LogicalQueryNodeProcessorFactory
    {
        public static ResultOfNode Run(LogicalQueryNode expression, ContextOfConverterFactToCG context)
        {
            var kind = expression.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    {
                        var node = new RelationNode(expression, context);
                        return node.Run();
                    }

                case KindOfLogicalQueryNode.Concept:
                    {
                        var node = new ConceptNode(expression, context);
                        return node.Run();
                    }

                case KindOfLogicalQueryNode.Value:
                    return Run(expression.Value, context);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private static ResultOfNode Run(Value value, ContextOfConverterFactToCG context)
        {
            var kindOfValue = value.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.ConditionalEntitySourceValue:
                    {
                        var node = new ConditionalEntitySourceValueNode(value.AsConditionalEntitySourceValue, context);
                        return node.Run();
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }
        }
    }
}
