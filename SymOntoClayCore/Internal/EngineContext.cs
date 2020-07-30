using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.InheritanceEngine;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.LogicalEngine;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.StandardLibrary;
using SymOntoClay.Core.Internal.States;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.TriggerExecution;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.Core.Internal.Threads;

namespace SymOntoClay.Core.Internal
{
    public class EngineContext : MainStorageContext, IEngineContext
    {
        public EngineContext(IEntityLogger logger)
            : base(logger)
        {
        }

        public CodeExecutorComponent CodeExecutor { get; set; }
        public TriggerExecutorComponent TriggerExecutor { get; set; }      
        public InstancesStorageComponent InstancesStorage { get; set; }
        public StatesStorageComponent StatesStorage { get; set; }       
        public LogicalEngineComponent LogicalEngine { get; set; }
        public InheritanceEngineComponent InheritanceEngine { get; set; }
        public StandardLibraryLoader StandardLibraryLoader { get; set; }       
        public ActivePeriodicObjectContext ActivePeriodicObjectContext { get; set; }
   
        ICodeExecutorComponent IEngineContext.CodeExecutor => CodeExecutor;
        ITriggerExecutorComponent IEngineContext.TriggerExecutor => TriggerExecutor;     
        ILogicalEngine IEngineContext.LogicalEngine => LogicalEngine;
        IInheritanceEngine IEngineContext.InheritanceEngine => InheritanceEngine;     
        IInstancesStorageComponent IEngineContext.InstancesStorage => InstancesStorage;
        IActivePeriodicObjectContext IEngineContext.ActivePeriodicObjectContext => ActivePeriodicObjectContext;
    }
}
