/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Core.Internal.Serialization;
using SymOntoClay.Core.Internal.Services;
using SymOntoClay.Core.Internal.StandardLibrary;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.Factories;
using SymOntoClay.Core.Internal.Htn;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.Threading;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class EngineContextHelper
    {
        public static EngineContext CreateAndInitContext(EngineSettings settings)
        {
            var context = new EngineContext(settings.MonitorNode);

            var baseContextComponents = new List<IBaseContextComponent>();

            BaseInitMainStorageContext(context, settings, KindOfStorage.Global, baseContextComponents);

            context.HostSupport = settings.HostSupport;
            context.HostListener = settings.HostListener;

            context.ConditionalEntityHostSupport = settings.ConditionalEntityHostSupport;
            context.SoundPublisherProvider = settings.SoundPublisherProvider;
            context.NLPConverterFactory = settings.NLPConverterFactory;

            var storageFactories = new StorageFactories(context);
            context.StorageFactories = storageFactories;
            baseContextComponents.Add(storageFactories);

            var standardLibraryLoader = new StandardLibraryLoader(context);
            context.StandardLibraryLoader = standardLibraryLoader;
            baseContextComponents.Add(standardLibraryLoader);

            var codeExecutor = new CodeExecutorComponent(context);
            context.CodeExecutor = codeExecutor;
            baseContextComponents.Add(codeExecutor);

            var htnExecutor = new HtnExecutorComponent(context, settings.HtnExecutionSettings);
            context.HtnExecutor = htnExecutor;
            baseContextComponents.Add(htnExecutor);

            var loaderFromSourceCode = new ActiveLoaderFromSourceCode(context);
            context.LoaderFromSourceCode = loaderFromSourceCode;
            baseContextComponents.Add(loaderFromSourceCode);

            var instancesStorage = new InstancesStorageComponent(context);
            context.InstancesStorage = instancesStorage;
            baseContextComponents.Add(instancesStorage);

            var threadingSettings = settings.ThreadingSettings?.CodeExecution;

            context.CodeExecutionThreadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                context.LinkedCancellationTokenSource.Token);

            context.TriggersThreadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                context.LinkedCancellationTokenSource.Token);

            InitComponents(baseContextComponents);

            return context;
        }

        public static void LoadFromSourceCode(EngineContext context)
        {
            context.Storage.LoadFromSourceCode(context);
            context.StandardLibraryLoader.LoadFromSourceCode();
            context.InstancesStorage.LoadFromSourceFiles();
            context.LoaderFromSourceCode.LoadFromSourceFiles();

            context.ServicesFactory.GetEntityConstraintsService().LoadFromSourceFiles();
        }

        public static void LoadFromSourceCode(MainStorageContext context)
        {
            context.Storage.LoadFromSourceCode();
            context.InstancesStorage.LoadFromSourceFiles();
            context.LoaderFromSourceCode.LoadFromSourceFiles();

            context.ServicesFactory.GetEntityConstraintsService().LoadFromSourceFiles();
        }

        public static MainStorageContext CreateAndInitMainStorageContext(StandaloneStorageSettings settings)
        {
            var context = new MainStorageContext(settings.MonitorNode);

            var baseContextComponents = new List<IBaseContextComponent>();

            BaseInitMainStorageContext(context, settings, settings.IsWorld ? KindOfStorage.World : KindOfStorage.Host, baseContextComponents);

            var instancesStorage = new BaseInstancesStorageComponent(context);
            context.InstancesStorage = instancesStorage;
            baseContextComponents.Add(instancesStorage);

            var loaderFromSourceCode = new BaseLoaderFromSourceCode(context);
            context.LoaderFromSourceCode = loaderFromSourceCode;
            baseContextComponents.Add(loaderFromSourceCode);

            InitComponents(baseContextComponents);

            return context;
        }

        public static BaseCoreContext CreateAndInitBaseCoreContext(BaseCoreSettings settings)
        {
            var context = new BaseCoreContext(settings.MonitorNode);

            var baseContextComponents = new List<IBaseContextComponent>();

            BaseInitBaseCoreContext(context, settings, baseContextComponents);

            InitComponents(baseContextComponents);

            return context;
        }

        public static void BaseInitMainStorageContext(MainStorageContext context, BaseStorageSettings settings, KindOfStorage kindGlobalOfStorage, List<IBaseContextComponent> baseContextComponents)
        {
            BaseInitBaseCoreContext(context, settings, baseContextComponents);

            context.Id = settings.Id;
            context.SelfName = NameHelper.CreateName(settings.Id);
            context.AppFile = settings.AppFile;

            context.LogicQueryParseAndCache = settings.LogicQueryParseAndCache;

            var storageComponentSettings = new StorageComponentSettings()
            {
                Categories = settings.Categories,
                EnableCategories = settings.EnableCategories
            };

            var storage = new StorageComponent(context, settings.ParentStorage, kindGlobalOfStorage, storageComponentSettings);
            context.Storage = storage;
            baseContextComponents.Add(storage);

            var parser = new Parser(context);
            context.Parser = parser;
            baseContextComponents.Add(parser);

            var compiler = new Compiler(context);
            context.Compiler = compiler;
            baseContextComponents.Add(compiler);

            var commonNamesStorage = new CommonNamesStorage(context);
            context.CommonNamesStorage = commonNamesStorage;
            baseContextComponents.Add(commonNamesStorage);

            var dataResolversFactory = new DataResolversFactory(context);
            context.DataResolversFactory = dataResolversFactory;
            baseContextComponents.Add(dataResolversFactory);

            var convertersFactory = new ConvertersFactory(context);
            context.ConvertersFactory = convertersFactory;
            baseContextComponents.Add(convertersFactory);

            var typeConverter = new TypeConverter(context);
            context.TypeConverter = typeConverter;
            baseContextComponents.Add(typeConverter);

            context.ActiveObjectContext = new ActiveObjectContext(settings.SyncContext, context.LinkedCancellationTokenSource.Token);
            context.ModulesStorage = settings.ModulesStorage;

            var servicesFactory = new ServicesFactory(context);
            context.ServicesFactory = servicesFactory;
            baseContextComponents.Add(servicesFactory);
        }

        private static void BaseInitBaseCoreContext(BaseCoreContext context, BaseCoreSettings settings, List<IBaseContextComponent> baseContextComponents)
        {
            context.DateTimeProvider = settings.DateTimeProvider;
            context.StandardFactsBuilder = settings.StandardFactsBuilder;

            context.CancellationTokenSource = new CancellationTokenSource();
            context.LinkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationTokenSource.Token, settings.CancellationToken);

            var asyncEventsThreadingSettings = settings.ThreadingSettings?.AsyncEvents;

            context.AsyncEventsThreadPool = new CustomThreadPool(asyncEventsThreadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                asyncEventsThreadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                context.LinkedCancellationTokenSource.Token);

            var garbageCollectionThreadingSettings = settings.ThreadingSettings?.GarbageCollection;

            context.GarbageCollectionThreadPool = new CustomThreadPool(garbageCollectionThreadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                garbageCollectionThreadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                context.LinkedCancellationTokenSource.Token);
        }

        public static void InitComponents(List<IBaseContextComponent> baseContextComponents)
        {
            foreach(var item in baseContextComponents)
            {
                item.LinkWithOtherBaseContextComponents();
            }

            foreach (var item in baseContextComponents)
            {
                item.Init();
            }
        }
    }
}
