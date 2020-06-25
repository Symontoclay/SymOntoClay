using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.InheritanceEngine;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.LogicalEngine;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.States;
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
        public EngineContext(IEntityLogger logger)
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

        public IEntityDictionary Dictionary { get; set; }

        public StorageComponent Storage { get; set; }
        public CodeExecutorComponent CodeExecutor { get; set; }
        public TriggerExecutorComponent TriggerExecutor { get; set; }
        public LoaderFromSourceCode LoaderFromSourceCode { get; set; }
        public Parser Parser { get; set; }
        public InstancesStorageComponent InstancesStorage { get; set; }
        public StatesStorageComponent StatesStorage { get; set; }
        public CommonNamesStorage CommonNamesStorage { get; set; }
        public LogicalEngineComponent LogicalEngine { get; set; }
        public InheritanceEngineComponent InheritanceEngine { get; set; }

        IStorageComponent IEngineContext.Storage => Storage;
        ICodeExecutorComponent IEngineContext.CodeExecutor => CodeExecutor;
        ITriggerExecutorComponent IEngineContext.TriggerExecutor => TriggerExecutor;
        ILoaderFromSourceCode IEngineContext.LoaderFromSourceCode => LoaderFromSourceCode;
        IParser IMainStorageContext.Parser => Parser;
        ICommonNamesStorage IEngineContext.CommonNamesStorage => CommonNamesStorage;
        ILogicalEngine IEngineContext.LogicalEngine => LogicalEngine;
        IInheritanceEngine IEngineContext.InheritanceEngine => InheritanceEngine;
    }
}
