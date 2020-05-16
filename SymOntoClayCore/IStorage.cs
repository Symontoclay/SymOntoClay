using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IStorage
    {
        KindOfStorage Kind { get; }
        ILogicalStorage LogicalStorage { get; }
        IMethodsStorage MethodsStorage { get; }
        ITriggersStorage TriggersStorage { get; }
        IInheritanceStorage InheritanceStorage { get; }
    }
}
