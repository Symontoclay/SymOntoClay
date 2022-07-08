using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class GlobalVisibleStorage : BaseLoggedComponent, IStorage
    {
        public GlobalVisibleStorage(IStorage globalStorage)
        {
        }

        private readonly object _lockObj = new object();

        #region IStorage
        /// <inheritdoc/>
        KindOfStorage IStorage.Kind => KindOfStorage.GlobalVisible;

        /// <inheritdoc/>
        ILogicalStorage IStorage.LogicalStorage => this;

        /// <inheritdoc/>
        IRelationsStorage IStorage.RelationsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IMethodsStorage IStorage.MethodsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IActionsStorage IStorage.ActionsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IStatesStorage IStorage.StatesStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        ITriggersStorage IStorage.TriggersStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IInheritanceStorage IStorage.InheritanceStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        ISynonymsStorage IStorage.SynonymsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IOperatorsStorage IStorage.OperatorsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IChannelsStorage IStorage.ChannelsStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IMetadataStorage IStorage.MetadataStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IVarStorage IStorage.VarStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        IFuzzyLogicStorage IStorage.FuzzyLogicStorage => throw new NotImplementedException();

        /// <inheritdoc/>
        void IStorage.AddParentStorage(IStorage storage) => throw new NotImplementedException();

        /// <inheritdoc/>
        void IStorage.RemoveParentStorage(IStorage storage) => throw new NotImplementedException();

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<StorageUsingOptions> result, IList<IStorage> usedStorages, int level, CollectChainOfStoragesOptions options)
        {
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
        }

        /// <inheritdoc/>
        void IStorage.CollectChainOfStorages(IList<IStorage> result) => throw new NotImplementedException();

        /// <inheritdoc/>
        IList<IStorage> IStorage.GetStorages() => throw new NotImplementedException();

        /// <inheritdoc/>
        DefaultSettingsOfCodeEntity IStorage.DefaultSettingsOfCodeEntity { get; set; }
#if DEBUG
        /// <inheritdoc/>
        void IStorage.DbgPrintFactsAndRules() => throw new NotImplementedException();
#endif
        #endregion

        private bool _isDisposed;

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock (_lockObj)
                {
                    return _isDisposed;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lockObj)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
            _realStorageContext.Dispose();
        }

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
