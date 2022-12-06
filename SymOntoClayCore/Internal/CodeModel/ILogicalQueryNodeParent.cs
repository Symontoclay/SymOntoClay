using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface ILogicalQueryNodeParent
    {
        void Remove(LogicalQueryNode node);
        void Replace(LogicalQueryNode oldNode, LogicalQueryNode newNode);
    }
}
