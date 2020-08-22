using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.InheritanceEngine;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.LogicalEngine;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Core.Internal.TriggerExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IEngineContext: IMainStorageContext
    {   
        ICodeExecutorComponent CodeExecutor { get; }
        ITriggerExecutorComponent TriggerExecutor { get; }
        ILogicalEngine LogicalEngine { get; }
        IInheritanceEngine InheritanceEngine { get; }
        IInstancesStorageComponent InstancesStorage { get; }
        IActivePeriodicObjectContext ActivePeriodicObjectContext { get; }
        IHostSupport HostSupport { get; }
    }
}
