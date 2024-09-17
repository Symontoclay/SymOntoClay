/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Storage.ActionsStoraging;
using SymOntoClay.Core.Internal.Storage.ChannelsStoraging;
using SymOntoClay.Core.Internal.Storage.ConstructorsStoraging;
using SymOntoClay.Core.Internal.Storage.FuzzyLogic;
using SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging;
using SymOntoClay.Core.Internal.Storage.InheritanceStoraging;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.Core.Internal.Storage.MetadataStoraging;
using SymOntoClay.Core.Internal.Storage.MethodsStoraging;
using SymOntoClay.Core.Internal.Storage.OperatorsStoraging;
using SymOntoClay.Core.Internal.Storage.RelationStoraging;
using SymOntoClay.Core.Internal.Storage.StatesStoraging;
using SymOntoClay.Core.Internal.Storage.SynonymsStoraging;
using SymOntoClay.Core.Internal.Storage.TriggersStoraging;
using SymOntoClay.Core.Internal.Storage.VarStoraging;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorageContext
    {
        public IMainStorageContext MainStorageContext { get; set; }
        public LogicalStorage LogicalStorage { get; set; }
        public RelationsStorage RelationsStorage { get; set; }
        public MethodsStorage MethodsStorage { get; set; }
        public ConstructorsStorage ConstructorsStorage { get; set; }
        public ActionsStorage ActionsStorage { get; set; }
        public StatesStorage StatesStorage { get; set; }
        public TriggersStorage TriggersStorage { get; set; }
        public InheritanceStorage InheritanceStorage { get; set; }
        public SynonymsStorage SynonymsStorage { get; set; }
        public OperatorsStorage OperatorsStorage { get; set; }
        public ChannelsStorage ChannelsStorage { get; set; }
        public MetadataStorage MetadataStorage { get; set; }
        public VarStorage VarStorage { get; set; }
        public FuzzyLogicStorage FuzzyLogicStorage { get; set; }
        public IdleActionItemsStorage IdleActionItemsStorage { get; set; }
        public RealStorage Storage { get; set; }
        public ILocalCodeExecutionContext ParentCodeExecutionContext { get; set; }
        public IList<IStorage> Parents { get; set; }
        public IInheritancePublicFactsReplicator InheritancePublicFactsReplicator { get; set; }
        public KindOfGC KindOfGC { get; set; }
        public bool EnableOnAddingFactEvent { get; set; }
        public bool Disabled { get; set; }

        public void EmitOnAddParentStorage(IMonitorLogger logger, IStorage storage)
        {
            OnAddParentStorage?.Invoke(storage);
        }

        public void EmitOnRemoveParentStorage(IMonitorLogger logger, IStorage storage)
        {
            OnRemoveParentStorage?.Invoke(storage);
        }

        [Obsolete("Serialization Refactoring", true)] public event Action<IStorage> OnAddParentStorage;
        [Obsolete("Serialization Refactoring", true)] public event Action<IStorage> OnRemoveParentStorage;

        public void Dispose()
        {
            LogicalStorage.Dispose();
            RelationsStorage.Dispose();
            MethodsStorage.Dispose();
            ConstructorsStorage.Dispose();
            ActionsStorage.Dispose();
            StatesStorage.Dispose();
            TriggersStorage.Dispose();
            InheritanceStorage.Dispose();
            SynonymsStorage.Dispose();
            OperatorsStorage.Dispose();
            ChannelsStorage.Dispose();
            MetadataStorage.Dispose();
            VarStorage.Dispose();
            FuzzyLogicStorage.Dispose();
            IdleActionItemsStorage.Dispose();
        }
    }
}
