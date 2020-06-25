using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.InheritanceEngine;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.LogicalEngine;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.States;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.TriggerExecution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class EngineContextHelper
    {
        public static EngineContext CreateAndInitContext(EngineSettings settings)
        {
            var context = new EngineContext(settings.Logger);

            context.Id = settings.Id;
            context.AppFile = settings.AppFile;
            context.Dictionary = settings.Dictionary;

            context.Storage = new StorageComponent(context, settings.ParentStorage);
            context.CodeExecutor = new CodeExecutorComponent(context);
            context.TriggerExecutor = new TriggerExecutorComponent(context);
            context.LoaderFromSourceCode = new LoaderFromSourceCode(context);
            context.Parser = new Parser(context);
            context.InstancesStorage = new InstancesStorageComponent(context);
            context.StatesStorage = new StatesStorageComponent(context);
            context.CommonNamesStorage = new CommonNamesStorage(context);
            context.LogicalEngine = new LogicalEngineComponent(context);
            context.InheritanceEngine = new InheritanceEngineComponent(context);

            return context;
        }

        public static void LoadFromSourceCode(EngineContext context)
        {
            context.CommonNamesStorage.LoadFromSourceCode();
            context.Storage.LoadFromSourceCode();
            context.StatesStorage.LoadFromSourceCode();
            context.LoaderFromSourceCode.LoadFromSourceFiles();
        }
    }
}
