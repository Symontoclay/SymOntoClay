/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Logging;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Services;
using SymOntoClay.Core.Internal.StandardLibrary;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class EngineContextHelper
    {
        public static EngineContext CreateAndInitContext(EngineSettings settings)
        {
            var context = new EngineContext(settings.Logger);

            BaseInitMainStorageContext(context, settings, KindOfStorage.Global);

            context.HostSupport = settings.HostSupport;
            context.HostListener = settings.HostListener;

            context.ConditionalEntityHostSupport = settings.ConditionalEntityHostSupport;
            context.SoundPublisherProvider = settings.SoundPublisherProvider;
            context.NLPConverterFactory = settings.NLPConverterFactory;

            context.StandardLibraryLoader = new StandardLibraryLoader(context);
            context.CodeExecutor = new CodeExecutorComponent(context);
            
            context.LoaderFromSourceCode = new ActiveLoaderFromSourceCode(context);
            context.InstancesStorage = new InstancesStorageComponent(context);

            return context;
        }

        public static void LoadFromSourceCode(EngineContext context)
        {
            context.CommonNamesStorage.LoadFromSourceCode();
            context.Storage.LoadFromSourceCode(context);
            context.StandardLibraryLoader.LoadFromSourceCode();
            context.InstancesStorage.LoadFromSourceFiles();
            context.LoaderFromSourceCode.LoadFromSourceFiles();

            context.ServicesFactory.GetEntityConstraintsService().Init();
        }

        public static void LoadFromSourceCode(MainStorageContext context)
        {
            context.CommonNamesStorage.LoadFromSourceCode();
            context.Storage.LoadFromSourceCode();
            context.InstancesStorage.LoadFromSourceFiles();
            context.LoaderFromSourceCode.LoadFromSourceFiles();

            context.ServicesFactory.GetEntityConstraintsService().Init();
        }

        public static MainStorageContext CreateAndInitMainStorageContext(StandaloneStorageSettings settings)
        {
            var context = new MainStorageContext(settings.Logger);

            BaseInitMainStorageContext(context, settings, settings.IsWorld ? KindOfStorage.World : KindOfStorage.Host );

            context.InstancesStorage = new BaseInstancesStorageComponent(context);
            context.LoaderFromSourceCode = new BaseLoaderFromSourceCode(context);

            return context;
        }

        public static BaseCoreContext CreateAndInitBaseCoreContext(BaseCoreSettings settings)
        {
            var context = new BaseCoreContext(settings.Logger);

            BaseInitBaseCoreContext(context, settings);

            return context;
        }

        public static void BaseInitMainStorageContext(MainStorageContext context, BaseStorageSettings settings, KindOfStorage kindGlobalOfStorage)
        {
            BaseInitBaseCoreContext(context, settings);

            context.Id = settings.Id;
            context.SelfName = NameHelper.CreateName(settings.Id);
            context.AppFile = settings.AppFile;

            context.LogicQueryParseAndCache = settings.LogicQueryParseAndCache;

            var storageComponentSettings = new StorageComponentSettings()
            {
                Categories = settings.Categories,
                EnableCategories = settings.EnableCategories
            };

            context.Storage = new StorageComponent(context, settings.ParentStorage, kindGlobalOfStorage, storageComponentSettings);

            context.Parser = new Parser(context);
            context.Compiler = new Compiler(context);
            context.CommonNamesStorage = new CommonNamesStorage(context);
            context.DataResolversFactory = new DataResolversFactory(context);
            context.ConvertersFactory = new ConvertersFactory(context);

            context.ActivePeriodicObjectContext = new ActivePeriodicObjectContext(settings.SyncContext);
            context.ModulesStorage = settings.ModulesStorage;

            context.LogicalSearchExplainProvider = new LoggingProvider(context, settings);

            context.ServicesFactory = new ServicesFactory(context);
        }

        private static void BaseInitBaseCoreContext(BaseCoreContext context, BaseCoreSettings settings)
        {
            context.DateTimeProvider = settings.DateTimeProvider;
        }
    }
}
