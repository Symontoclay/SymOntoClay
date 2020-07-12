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
            : base(settings.Logger)
        {
#if DEBUG
            //Log($"kind = {kind}");
            //Log($"settings = {settings}");
#endif

            _kind = kind;
            _realStorageContext = new RealStorageContext();
            _realStorageContext.Storage = this;
            _realStorageContext.Logger = settings.Logger;
            _realStorageContext.EntityDictionary = settings.EntityDictionary;
            _realStorageContext.Compiler = settings.Compiler;
            _realStorageContext.CommonNamesStorage = settings.CommonNamesStorage;

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
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

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
        void IStorage.CollectChainOfStorages(IList<KeyValuePair<uint, IStorage>> result, uint level)
        {
#if DEBUG
            //Log($"result?.Count = {result?.Count}");
            //Log($"level = {level}");
#endif

            level++;

            result.Add(new KeyValuePair<uint, IStorage>(level, this));

            var parentsList = _realStorageContext.Parents;

            if (parentsList.Any())
            {
                foreach(var parent in parentsList)
                {
                    parent.CollectChainOfStorages(result, level);
                }
            }
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
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
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
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
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
            sb.AppendLine($"{spaces}{nameof(Kind)} = {_kind}");
            return sb.ToString();
        }
    }
}
