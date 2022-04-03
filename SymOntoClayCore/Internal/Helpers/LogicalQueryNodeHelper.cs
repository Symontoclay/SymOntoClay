using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class LogicalQueryNodeHelper
    {
        public static Value ToValue(LogicalQueryNode node)
        {
            var kindOfNode = node.Kind;

            switch (kindOfNode)
            {
                case KindOfLogicalQueryNode.Entity:
                case KindOfLogicalQueryNode.Concept:
                    return node.Name;

                case KindOfLogicalQueryNode.Value:
                    return node.Value;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfNode), kindOfNode, null);
            }
        }
    }
}
