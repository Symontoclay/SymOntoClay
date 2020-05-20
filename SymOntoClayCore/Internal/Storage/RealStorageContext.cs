using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorageContext
    {
        public IEntityLogger Logger { get; set; }
        public IEntityDictionary EntityDictionary { get; set; }
        public LogicalStorage.LogicalStorage LogicalStorage { get; set; }
        public MethodsStorage.MethodsStorage MethodsStorage { get; set; }
        public TriggersStorage.TriggersStorage TriggersStorage { get; set; }
        public InheritanceStorage.InheritanceStorage InheritanceStorage { get; set; }
        public SynonymsStorage.SynonymsStorage SynonymsStorage { get; set; }
        public IList<IStorage> Parents { get; set; }
    }
}
