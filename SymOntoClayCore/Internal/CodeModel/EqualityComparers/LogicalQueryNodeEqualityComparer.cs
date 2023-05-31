using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.EqualityComparers
{
    public class LogicalQueryNodeEqualityComparer : IEqualityComparer<LogicalQueryNode>
    {
        /// <inheritdoc/>
        public bool Equals(LogicalQueryNode x, LogicalQueryNode y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if(x.Kind != y.Kind)
            {
                return false;
            }

            var kind = x.Kind;

            switch (kind)
            {
                case KindOfLogicalQueryNode.Value:
                    return x.Value.Equals(y.Value);

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                    return x.Name == y.Name;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        /// <inheritdoc/>
        public int GetHashCode(LogicalQueryNode obj)
        {
            var kind = obj.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                    return kind.GetHashCode() ^ obj.Name.GetHashCode();

                case KindOfLogicalQueryNode.Value:
                    return kind.GetHashCode() ^ obj.Value.GetSystemValue().GetHashCode();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
