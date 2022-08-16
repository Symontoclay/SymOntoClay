/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.TypesConvertors;
using System.Collections.Generic;
using System.Numerics;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IWorldCoreGameComponentContext
    {
        void AddGameComponent(IGameComponent component);
        void AddPublicFactsStorage(IGameComponent component);
        void RemoveGameComponent(IGameComponent component);

        bool CanBeTakenBy(int instanceId, IEntity subject);
        Vector3? GetPosition(int instanceId);

        IList<int> AvailableInstanceIdList { get; }
        IStorage GetPublicFactsStorageByInstanceId(int instanceId);
        string GetIdForFactsByInstanceId(int instanceId);
        int GetInstanceIdByIdForFacts(string id);
        IEntityLogger CreateLogger(string name);
        IActivePeriodicObjectCommonContext SyncContext { get; }
        
        IModulesStorage ModulesStorage { get; }
        IStandaloneStorage StandaloneStorage { get; }
        string TmpDir { get; }
        IPlatformTypesConvertorsRegistry PlatformTypesConvertors { get; }
        IInvokerInMainThread InvokerInMainThread { get; }
        IDateTimeProvider DateTimeProvider { get; }
        ILogicQueryParseAndCache LogicQueryParseAndCache { get; }
        ISoundBus SoundBus { get; }
        INLPConverterFactory NLPConverterFactory { get; }
        IStandardFactsBuilder StandardFactsBuilder { get; }
        KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; }
        string LogicalSearchExplainDumpDir { get; }
        bool EnableAddingRemovingFactLoggingInStorages { get; }
    }
}
