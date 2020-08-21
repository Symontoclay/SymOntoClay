using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IWorldCoreGameComponentContext
    {
        void AddGameComponent(IGameComponent component);
        void RemoveGameComponent(IGameComponent component);
        IEntityLogger CreateLogger(string name);
        IActivePeriodicObjectCommonContext SyncContext { get; }
        SymOntoClay.Core.IEntityDictionary SharedDictionary { get; }
        IModulesStorage ModulesStorage { get; }
        IStandaloneStorage StandaloneStorage { get; }
        string TmpDir { get; }
        IPlatformTypesConvertorsRegistry PlatformTypesConvertors { get; }
        IInvokerInMainThread InvokerInMainThread { get; }
        IDateTimeProvider DateTimeProvider { get; }
    }
}
