/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using System.Collections.Generic;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IWorldCoreGameComponentContext
    {
        void AddGameComponent(IGameComponent component);
        void RemoveGameComponent(IGameComponent component);
        IList<int> AvailableInstanceIdList { get; }
        IStorage GetPublicFactsStorageByInstanceId(int instanceId);
        string GetIdForFactsByInstanceId(int instanceId);
        IEntityLogger CreateLogger(string name);
        IActivePeriodicObjectCommonContext SyncContext { get; }
        IEntityDictionary SharedDictionary { get; }
        IModulesStorage ModulesStorage { get; }
        IStandaloneStorage StandaloneStorage { get; }
        string TmpDir { get; }
        IPlatformTypesConvertorsRegistry PlatformTypesConvertors { get; }
        IInvokerInMainThread InvokerInMainThread { get; }
        IDateTimeProvider DateTimeProvider { get; }
        ILogicQueryParseAndCache LogicQueryParseAndCache { get; }        
    }
}
