/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
