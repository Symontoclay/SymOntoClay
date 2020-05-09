using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.TriggerExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public class EngineContext : BaseComponent, IEngineContext
    {
        public EngineContext(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets app file.
        /// </summary>
        public string AppFile { get; set; }

        public StorageComponent Storage { get; set; }
        public CodeExecutorComponent CodeExecutor { get; set; }
        public TriggerExecutorComponent TriggerExecutor { get; set; }
        public LoaderFromSourceCode LoaderFromSourceCode { get; set; }
        public Parser Parser { get; set; }

        IStorageComponent IEngineContext.Storage => Storage;
        ICodeExecutorComponent IEngineContext.CodeExecutor => CodeExecutor;
        ITriggerExecutorComponent IEngineContext.TriggerExecutor => TriggerExecutor;
        ILoaderFromSourceCode IEngineContext.LoaderFromSourceCode => LoaderFromSourceCode;
        IParser IBaseLoaderFromSourceCodeContext.Parser => Parser;
    }
}
