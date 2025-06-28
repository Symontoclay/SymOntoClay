using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.InternalRunners
{
    public class LeaveLifecycleTriggersRunner: BaseLifecycleTriggersRunner
    {
        public LeaveLifecycleTriggersRunner(IMonitorLogger logger, IEngineContext context, IInstance instance, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator, IStorage storage)
            : base(logger: logger, context: context, instance: instance, holder: holder, localCodeExecutionContext: localCodeExecutionContext, executionCoordinator: executionCoordinator, storage: storage, kindOfSystemEvent: KindOfSystemEventOfInlineTrigger.Leave, normalOrder: false, runOnce: true)
        {
        }
    }
}
