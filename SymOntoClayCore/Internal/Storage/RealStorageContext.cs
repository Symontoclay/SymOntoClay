/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Storage.FuzzyLogic;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorageContext
    {
        public IMainStorageContext MainStorageContext { get; set; }
        public LogicalStorage.LogicalStorage LogicalStorage { get; set; }
        public MethodsStorage.MethodsStorage MethodsStorage { get; set; }
        public ActionsStorage.ActionsStorage ActionsStorage { get; set; }
        public TriggersStorage.TriggersStorage TriggersStorage { get; set; }
        public InheritanceStorage.InheritanceStorage InheritanceStorage { get; set; }
        public SynonymsStorage.SynonymsStorage SynonymsStorage { get; set; }
        public OperatorsStorage.OperatorsStorage OperatorsStorage { get; set; }
        public ChannelsStorage.ChannelsStorage ChannelsStorage { get; set; }
        public MetadataStorage.MetadataStorage MetadataStorage { get; set; }
        public VarStorage.VarStorage VarStorage { get; set; }
        public FuzzyLogicStorage FuzzyLogicStorage { get; set; }
        public RealStorage Storage { get; set; }
        public IList<IStorage> Parents { get; set; }
        public IInheritancePublicFactsReplicator InheritancePublicFactsReplicator { get; set; }

        public void EmitOnAddParentStorage(IStorage storage)
        {
            OnAddParentStorage?.Invoke(storage);
        }

        public void EmitOnRemoveParentStorage(IStorage storage)
        {
            OnRemoveParentStorage?.Invoke(storage);
        }

        public event Action<IStorage> OnAddParentStorage;
        public event Action<IStorage> OnRemoveParentStorage;
    }
}
