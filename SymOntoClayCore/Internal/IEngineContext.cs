using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.TriggerExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IEngineContext
    {
        string Id { get; }
        string AppFile { get; }

        ILogger Logger { get; }
        IStorageComponent Storage { get; }
        ICodeExecutorComponent CodeExecutor { get; }
        ITriggerExecutorComponent TriggerExecutor { get; }
    }
}
