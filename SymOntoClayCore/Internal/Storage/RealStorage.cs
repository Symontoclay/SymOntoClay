using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorage : BaseLoggedComponent, IStorage
    {
        public RealStorage(KindOfStorage kind, IEntityLogger logger, IEntityDictionary entityDictionary)
            : base(logger)
        {
            _kind = kind;
            _realStorageContext = new RealStorageContext();
            _realStorageContext.Logger = logger;
            _realStorageContext.EntityDictionary = entityDictionary;

            _logicalStorage = new LogicalStorage.LogicalStorage(_kind, _realStorageContext);
            _realStorageContext.LogicalStorage = _logicalStorage;
            _methodsStorage = new MethodsStorage.MethodsStorage(_kind, _realStorageContext);
            _realStorageContext.MethodsStorage = _methodsStorage;
            _triggersStorage = new TriggersStorage.TriggersStorage(_kind, _realStorageContext);
            _realStorageContext.TriggersStorage = _triggersStorage;
            _inheritanceStorage = new InheritanceStorage.InheritanceStorage(_kind, _realStorageContext);
            _realStorageContext.InheritanceStorage = _inheritanceStorage;

        }

        private readonly KindOfStorage _kind;

        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        private LogicalStorage.LogicalStorage _logicalStorage;

        public ILogicalStorage LogicalStorage => _logicalStorage;

        private MethodsStorage.MethodsStorage _methodsStorage;
        private TriggersStorage.TriggersStorage _triggersStorage;
        private InheritanceStorage.InheritanceStorage _inheritanceStorage;

        public IMethodsStorage MethodsStorage => _methodsStorage;
        public ITriggersStorage TriggersStorage => _triggersStorage;
        public IInheritanceStorage InheritanceStorage => _inheritanceStorage;
    }
}
