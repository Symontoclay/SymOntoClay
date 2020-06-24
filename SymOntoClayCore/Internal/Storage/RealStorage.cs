using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorage : BaseLoggedComponent, IStorage
    {
        public RealStorage(KindOfStorage kind, RealStorageSettings settings)
            : base(settings.Logger)
        {
#if DEBUG
            Log($"kind = {kind}");
            Log($"settings = {settings}");
#endif

            _kind = kind;
            _realStorageContext = new RealStorageContext();
            _realStorageContext.Logger = settings.Logger;
            _realStorageContext.EntityDictionary = settings.EntityDictionary;

            var parents = settings.ParentsStorages;

            if(parents == null)
            {
                _realStorageContext.Parents = new List<IStorage>();
            }
            else
            {
                _realStorageContext.Parents = parents;
            }

            _logicalStorage = new LogicalStorage.LogicalStorage(_kind, _realStorageContext);
            _realStorageContext.LogicalStorage = _logicalStorage;
            _methodsStorage = new MethodsStorage.MethodsStorage(_kind, _realStorageContext);
            _realStorageContext.MethodsStorage = _methodsStorage;
            _triggersStorage = new TriggersStorage.TriggersStorage(_kind, _realStorageContext);
            _realStorageContext.TriggersStorage = _triggersStorage;
            _inheritanceStorage = new InheritanceStorage.InheritanceStorage(_kind, _realStorageContext);
            _realStorageContext.InheritanceStorage = _inheritanceStorage;
            _synonymsStorage = new SynonymsStorage.SynonymsStorage(_kind, _realStorageContext);
            _realStorageContext.SynonymsStorage = _synonymsStorage;
            _operatorsStorage = new OperatorsStorage.OperatorsStorage(_kind, _realStorageContext);
            _realStorageContext.OperatorsStorage = _operatorsStorage;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        private LogicalStorage.LogicalStorage _logicalStorage;

        /// <inheritdoc/>
        public ILogicalStorage LogicalStorage => _logicalStorage;

        private MethodsStorage.MethodsStorage _methodsStorage;
        private TriggersStorage.TriggersStorage _triggersStorage;
        private InheritanceStorage.InheritanceStorage _inheritanceStorage;

        /// <inheritdoc/>
        public IMethodsStorage MethodsStorage => _methodsStorage;

        /// <inheritdoc/>
        public ITriggersStorage TriggersStorage => _triggersStorage;

        /// <inheritdoc/>
        public IInheritanceStorage InheritanceStorage => _inheritanceStorage;
   
        private SynonymsStorage.SynonymsStorage _synonymsStorage;

        /// <inheritdoc/>
        public ISynonymsStorage SynonymsStorage => _synonymsStorage;

        private OperatorsStorage.OperatorsStorage _operatorsStorage;

        /// <inheritdoc/>
        public IOperatorsStorage OperatorsStorage => _operatorsStorage;

        /// <inheritdoc/>
        IStorage IStorage.GetConsolidatedStorage()
        {
#if DEBUG
            Log("Do");
#endif

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        void IStorage.CollectParents(IList<IStorage> result, uint level)
        {
#if DEBUG
            Log($"result?.Count = {result?.Count}");
            Log($"level = {level}");
#endif

            throw new NotImplementedException();
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
