using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IStorage : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        KindOfStorage Kind { get; }
        ILogicalStorage LogicalStorage { get; }
        IMethodsStorage MethodsStorage { get; }
        ITriggersStorage TriggersStorage { get; }
        IInheritanceStorage InheritanceStorage { get; }
        IStorage GetConsolidatedStorage();
        void CollectParents(IList<IStorage> result, uint level);
    }
}
