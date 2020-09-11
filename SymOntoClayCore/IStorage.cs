using SymOntoClay.Core.Internal;
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
        ISynonymsStorage SynonymsStorage { get; }
        IOperatorsStorage OperatorsStorage { get; }
        IChannelsStorage ChannelsStorage { get; }
        IMetadataStorage MetadataStorage { get; }
        IVarStorage VarStorage { get; }
        void CollectChainOfStorages(IList<StorageUsingOptions> result, int level);
        DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get; set; }
    }
}
