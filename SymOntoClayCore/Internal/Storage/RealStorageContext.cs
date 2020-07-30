﻿using SymOntoClay.Core.Internal.Compiling;
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
        public TriggersStorage.TriggersStorage TriggersStorage { get; set; }
        public InheritanceStorage.InheritanceStorage InheritanceStorage { get; set; }
        public SynonymsStorage.SynonymsStorage SynonymsStorage { get; set; }
        public OperatorsStorage.OperatorsStorage OperatorsStorage { get; set; }
        public ChannelsStorage.ChannelsStorage ChannelsStorage { get; set; }
        public MetadataStorage.MetadataStorage MetadataStorage { get; set; }
        public VarStorage.VarStorage VarStorage { get; set; }
        public RealStorage Storage { get; set; }
        public IList<IStorage> Parents { get; set; }
    }
}
