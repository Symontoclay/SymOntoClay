using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.ConvertingFactToInternalCG
{
    public static class LogicalQueryNodeProcessorFactory
    {
        public static ResultOfNode Run(LogicalQueryNode expression, ContextOfConverterFactToInternalCG context)
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

                case KindOfLogicalQueryNode.LogicalVar:
                    {
                        var node = new LogicalVarNode(expression, context);
                        return node.Run();
                    }

                case KindOfLogicalQueryNode.BinaryOperator:
                    {
                        var kindOfOperator = expression.KindOfOperator;

                        switch (kindOfOperator)
                        {
                            case KindOfOperatorOfLogicalQueryNode.And:
                                {
                                    var node = new BinaryOperatorAndNode(expression, context);
                                    return node.Run();
                                }

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
                        }
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private static ResultOfNode Run(Value value, ContextOfConverterFactToInternalCG context)
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
