using SymOntoClay.Core;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IWorldCoreGameComponentContext
    {
        void AddGameComponent(IGameComponent component);
        void RemoveGameComponent(IGameComponent component);
        ILogger CreateLogger(string name);
        ISyncContext SyncContext { get; }
        SymOntoClay.Core.IDictionary SharedDictionary { get; }
        IModulesStorage ModulesStorage { get; }
        IStandaloneStorage StandaloneStorage { get; }
    }
}
