/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface IStorage : ISymOntoClayDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        KindOfStorage Kind { get; }
        StrongIdentifierValue TargetClassName { get; }
        StrongIdentifierValue InstanceName { get; }
        IInstance Instance { get; }

        bool IsIsolated { get; }

        ILogicalStorage LogicalStorage { get; }
        IRelationsStorage RelationsStorage { get; }
        IMethodsStorage MethodsStorage { get; }
        IConstructorsStorage ConstructorsStorage { get; }
        IActionsStorage ActionsStorage { get; }
        IStatesStorage StatesStorage { get; }
        ITriggersStorage TriggersStorage { get; }
        IInheritanceStorage InheritanceStorage { get; }
        ISynonymsStorage SynonymsStorage { get; }
        IOperatorsStorage OperatorsStorage { get; }
        IChannelsStorage ChannelsStorage { get; }
        IMetadataStorage MetadataStorage { get; }
        IVarStorage VarStorage { get; }
        IFuzzyLogicStorage FuzzyLogicStorage { get; }
        IIdleActionItemsStorage IdleActionItemsStorage { get; }
        ITasksStorage TasksStorage { get; }
        IPropertyStorage PropertyStorage { get; }
        
        void AddParentStorage(IMonitorLogger logger, IStorage storage);
        void RemoveParentStorage(IMonitorLogger logger, IStorage storage);
        void CollectChainOfStorages(IMonitorLogger logger, IList<StorageUsingOptions> result, IList<IStorage> usedStorages, int level, CollectChainOfStoragesOptions options);
        void CollectChainOfStorages(IMonitorLogger logger, IList<IStorage> result);
        IList<IStorage> GetStorages(IMonitorLogger logger);
        DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get; set; }

        List<StorageUsingOptions> CodeItemsStoragesList { get; set; }

        void AddOnParentStorageChangedHandler(IOnParentStorageChangedStorageHandler handler);
        void RemoveOnParentStorageChangedHandler(IOnParentStorageChangedStorageHandler handler);

        string FactsAndRulesToDbgString();

#if DEBUG
        void DbgPrintFactsAndRules(IMonitorLogger logger);
#endif
    }
}
