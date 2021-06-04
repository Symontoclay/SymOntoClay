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

using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage.FuzzyLogic;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorage : BaseLoggedComponent, IStorage
    {
        public RealStorage(KindOfStorage kind, RealStorageSettings settings)
            : base(settings.MainStorageContext.Logger)
        {
#if DEBUG
            //Log($"kind = {kind}");
            //Log($"settings = {settings}");
#endif

            _kind = kind;
            _realStorageContext = new RealStorageContext();
            _realStorageContext.Storage = this;
            _realStorageContext.MainStorageContext = settings.MainStorageContext;
            _realStorageContext.InheritancePublicFactsReplicator = settings.InheritancePublicFactsReplicator;

            DefaultSettingsOfCodeEntity = settings.DefaultSettingsOfCodeEntity;

            var parents = settings.ParentsStorages;

            if(parents == null)
            {
                _realStorageContext.Parents = new List<IStorage>();
            }
            else
            {
                _realStorageContext.Parents = parents;
            }

            _realStorageContext.LogicalStorage = new LogicalStorage.LogicalStorage(_kind, _realStorageContext);
            _realStorageContext.MethodsStorage = new MethodsStorage.MethodsStorage(_kind, _realStorageContext);
            _realStorageContext.TriggersStorage = new TriggersStorage.TriggersStorage(_kind, _realStorageContext);
            _realStorageContext.InheritanceStorage = new InheritanceStorage.InheritanceStorage(_kind, _realStorageContext);
            _realStorageContext.SynonymsStorage = new SynonymsStorage.SynonymsStorage(_kind, _realStorageContext);
            _realStorageContext.OperatorsStorage = new OperatorsStorage.OperatorsStorage(_kind, _realStorageContext);
            _realStorageContext.ChannelsStorage = new ChannelsStorage.ChannelsStorage(_kind, _realStorageContext);
            _realStorageContext.MetadataStorage = new MetadataStorage.MetadataStorage(_kind, _realStorageContext);
            _realStorageContext.VarStorage = new VarStorage.VarStorage(_kind, _realStorageContext);
            _realStorageContext.FuzzyLogicStorage = new FuzzyLogicStorage(_kind, _realStorageContext);
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;
        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public ILogicalStorage LogicalStorage => _realStorageContext.LogicalStorage;

        /// <inheritdoc/>
        public IMethodsStorage MethodsStorage => _realStorageContext.MethodsStorage;

        /// <inheritdoc/>
        public ITriggersStorage TriggersStorage => _realStorageContext.TriggersStorage;

        /// <inheritdoc/>
        public IInheritanceStorage InheritanceStorage => _realStorageContext.InheritanceStorage;
   
        /// <inheritdoc/>
        public ISynonymsStorage SynonymsStorage => _realStorageContext.SynonymsStorage;

        /// <inheritdoc/>
        public IOperatorsStorage OperatorsStorage => _realStorageContext.OperatorsStorage;

        /// <inheritdoc/>
        public IChannelsStorage ChannelsStorage => _realStorageContext.ChannelsStorage;

        /// <inheritdoc/>
        public IMetadataStorage MetadataStorage => _realStorageContext.MetadataStorage;

        /// <inheritdoc/>
        public IVarStorage VarStorage => _realStorageContext.VarStorage;

        /// <inheritdoc/>
        public IFuzzyLogicStorage FuzzyLogicStorage => _realStorageContext.FuzzyLogicStorage;

        /// <inheritdoc/>
        public void AddParentStorage(IStorage storage)
        {
            lock (_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if(parentsList.Contains(storage))
                {
                    return;
                }
                
                parentsList.Add(storage);

                _realStorageContext.EmitOnAddParentStorage(storage);
            }
        }

        /// <inheritdoc/>
        public void RemoveParentStorage(IStorage storage)
        {
            lock (_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if (parentsList.Contains(storage))
                {
                    parentsList.Remove(storage);

                    _realStorageContext.EmitOnRemoveParentStorage(storage);
                }
            }
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<StorageUsingOptions> result, IList<IStorage> usedStorages, int level, CollectChainOfStoragesOptions options)
        {
#if DEBUG
            //Log($"result?.Count = {result?.Count}");
            //Log($"level = {level}");
#endif

            if (usedStorages.Contains(this))
            {
                return;
            }

            usedStorages.Add(this);

            //if(result.Any(p => p.Storage == this))
            //{
            //    return;
            //}

            level++;

            var item = new StorageUsingOptions()
            {
                Priority = level,
                Storage = this,
                UseFacts = true,
                UseProductions = true,
                UseAdditionalInstances = true
            };

            if (options != null)
            {
                if (options.UseFacts.HasValue)
                {
                    item.UseFacts = options.UseFacts.Value;
                }
            }

            result.Add(item);

            lock(_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if (parentsList.Any())
                {
                    foreach (var parent in parentsList)
                    {
                        parent.CollectChainOfStorages(result, usedStorages, level, options);
                    }
                }
            }
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<IStorage> result)
        {
            CollectChainOfStorages(result);
        }

        private void CollectChainOfStorages(IList<IStorage> result)
        {
            if(result.Contains(this))
            {
                return;
            }

            result.Add(this);

            lock (_lockObj)
            {
                var parentsList = _realStorageContext.Parents;

                if (parentsList.Any())
                {
                    foreach (var parent in parentsList)
                    {
                        parent.CollectChainOfStorages(result);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IList<IStorage> GetStorages()
        {
            var result = new List<IStorage>();

            CollectChainOfStorages(result);

            return result;
        }

        /// <inheritdoc/>
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get; set; }

#if DEBUG
        /// <inheritdoc/>
        public void DbgPrintFactsAndRules()
        {
            LogicalStorage.DbgPrintFactsAndRules();
        }
#endif

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            sb.AppendLine($"{spaces}Owner = {_realStorageContext.MainStorageContext.Id}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            sb.AppendLine($"{spaces}Owner = {_realStorageContext.MainStorageContext.Id}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}HashCode = {GetHashCode()}");
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            sb.AppendLine($"{spaces}Owner = {_realStorageContext.MainStorageContext.Id}");
            return sb.ToString();
        }
    }
}
