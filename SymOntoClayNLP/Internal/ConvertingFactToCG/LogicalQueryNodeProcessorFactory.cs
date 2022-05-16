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

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
