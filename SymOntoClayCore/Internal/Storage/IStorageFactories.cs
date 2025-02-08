using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Storage.Factories;

namespace SymOntoClay.Core.Internal.Storage
{
    public interface IStorageFactories
    {
        IStorageFactory AppInstanceStorageFactory { get; }
        IStorageFactory ObjectStorageFactory { get; }
        IStorageFactory StateStorageFactory { get; }
        IStorageFactory ActionStorageFactory { get; }
        IStorageFactory RootTaskInstanceStorageFactory { get; }
        IStorageFactory StrategicTaskInstanceStorageFactory { get; }
        IStorageFactory TacticalTaskInstanceStorageFactory { get; }
        IStorageFactory CompoundTaskInstanceStorageFactory { get; }
    }
}
