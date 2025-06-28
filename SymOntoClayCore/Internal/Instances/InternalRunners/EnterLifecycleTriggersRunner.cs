using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.InternalRunners
{
    public class EnterLifecycleTriggersRunner: BaseLifecycleTriggersRunner
    {
        public EnterLifecycleTriggersRunner(IMonitorLogger logger, IEngineContext context, IInstance instance, StrongIdentifierValue holder, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator, IStorage storage)
            : base(logger: logger, context: context, instance: instance, holder: holder, localCodeExecutionContext: localCodeExecutionContext, executionCoordinator: executionCoordinator, storage: storage, kindOfSystemEvent: KindOfSystemEventOfInlineTrigger.Enter, normalOrder: true, runOnce: true)
        {
        }
    }
}
