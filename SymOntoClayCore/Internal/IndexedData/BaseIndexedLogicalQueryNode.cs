using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public abstract class BaseIndexedLogicalQueryNode : IndexedAnnotatedItem
    {
        public abstract KindOfLogicalQueryNode Kind { get; }
        public abstract KindOfOperatorOfLogicalQueryNode KindOfOperator { get; }
    }
}
