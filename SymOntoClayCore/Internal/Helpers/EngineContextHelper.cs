/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeExecution;
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
using System;
using System.Collections.Generic;
using System.Text;
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
            context.StandardLibraryLoader = new StandardLibraryLoader(context);
            context.CodeExecutor = new CodeExecutorComponent(context);
            context.TriggerExecutor = new TriggerExecutorComponent(context);
            context.LoaderFromSourceCode = new ActiveLoaderFromSourceCode(context);
            context.InstancesStorage = new InstancesStorageComponent(context);
            context.StatesStorage = new StatesStorageComponent(context);
            context.LogicalEngine = new LogicalEngineComponent(context);
            context.InheritanceEngine = new InheritanceEngineComponent(context);
            context.ActivePeriodicObjectContext = new ActivePeriodicObjectContext(settings.SyncContext);

            return context;
        }

        public static void LoadFromSourceCode(EngineContext context)
        {
            context.CommonNamesStorage.LoadFromSourceCode();
            context.Storage.LoadFromSourceCode();
            context.StandardLibraryLoader.LoadFromSourceCode();
            context.StatesStorage.LoadFromSourceCode();
            context.InstancesStorage.LoadFromSourceFiles();
            context.LoaderFromSourceCode.LoadFromSourceFiles();
        }

        public static void LoadFromSourceCode(MainStorageContext context)
        {
            context.CommonNamesStorage.LoadFromSourceCode();
            context.Storage.LoadFromSourceCode();            
            context.LoaderFromSourceCode.LoadFromSourceFiles();
        }

        public static MainStorageContext CreateAndInitMainStorageContext(StandaloneStorageSettings settings)
        {
            var context = new MainStorageContext(settings.Logger);

            BaseInitMainStorageContext(context, settings, settings.IsWorld ? KindOfStorage.World : KindOfStorage.Host );

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
            context.AppFile = settings.AppFile;

            context.LogicQueryParseAndCache = settings.LogicQueryParseAndCache;

            context.Storage = new StorageComponent(context, settings.ParentStorage, kindGlobalOfStorage);
            context.Parser = new Parser(context);
            context.Compiler = new Compiler(context);
            context.CommonNamesStorage = new CommonNamesStorage(context);
            context.DataResolversFactory = new DataResolversFactory(context);            
        }

        private static void BaseInitBaseCoreContext(BaseCoreContext context, BaseCoreSettings settings)
        {
            context.Dictionary = settings.Dictionary;
        }
    }
}
