/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Helpers
{
    public static class TstEngineContextHelper
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public static TstComplexContext CreateAndInitContext()
        {
            var invokingInMainThread = TstInvokerInMainThreadFactory.Create();

            var result = new TstComplexContext();

            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "NpcLogs");

            var worldSettings = new WorldSettings();

            worldSettings.SharedModulesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            worldSettings.ImagesRootDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            worldSettings.TmpDir = Path.Combine(Directory.GetCurrentDirectory(), "Tmp");

            worldSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\HelloWorld.world");

            worldSettings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { ConsoleLogger.Instance/*, CommonNLogLogger.Instance */},
                Enable = true,
                EnableRemoteConnection = true
            };

            worldSettings.InvokerInMainThread = invokingInMainThread;

            _logger.Log($"worldSettings = {worldSettings}");

            var worldContext = new WorldContext();
            worldContext.SetSettings(worldSettings);

            var npcSettings = new BipedNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PeaceKeeper\PeaceKeeper.npc");
            npcSettings.PlatformSupport = new TstPlatformSupport();

            _logger.Log($"npcSettings = {npcSettings}");

            var entityLogger = new LoggerImpementation();

            var tmpDir = Path.Combine(worldSettings.TmpDir, npcSettings.Id);

            Directory.CreateDirectory(worldSettings.TmpDir);

            var standaloneStorageSettings = new StandaloneStorageSettings();
            standaloneStorageSettings.Id = npcSettings.Id;
            standaloneStorageSettings.IsWorld = false;
            standaloneStorageSettings.AppFile = npcSettings.HostFile;
            standaloneStorageSettings.Logger = entityLogger;
            standaloneStorageSettings.Dictionary = worldContext.SharedDictionary.Dictionary;
            standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage.ModulesStorage;
            standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage.StandaloneStorage;

#if DEBUG
            _logger.Log($"standaloneStorageSettings = {standaloneStorageSettings}");
#endif
            var _hostStorage = new StandaloneStorage(standaloneStorageSettings);

            var coreEngineSettings = new EngineSettings();
            coreEngineSettings.Id = npcSettings.Id;
            coreEngineSettings.AppFile = npcSettings.LogicFile;
            coreEngineSettings.Logger = entityLogger;
            coreEngineSettings.SyncContext = worldContext.ThreadsComponent;
            coreEngineSettings.Dictionary = worldContext.SharedDictionary.Dictionary;
            coreEngineSettings.ModulesStorage = worldContext.ModulesStorage.ModulesStorage;
            coreEngineSettings.ParentStorage = _hostStorage;
            coreEngineSettings.TmpDir = tmpDir;
            coreEngineSettings.HostSupport = new TstHostSupportComponent(npcSettings.PlatformSupport);

#if DEBUG
            _logger.Log($"coreEngineSettings = {coreEngineSettings}");
#endif

#if DEBUG
            //_logger.Log($"Begin worldContext.Start()");
#endif

            worldContext.Start();

            result.WorldContext = worldContext;

#if DEBUG
            //_logger.Log($"After worldContext.Start()");
#endif

            var context = EngineContextHelper.CreateAndInitContext(coreEngineSettings);

#if DEBUG
            //_logger.Log($"After var context = EngineContextHelper.CreateAndInitContext(coreEngineSettings);");
#endif

            context.CommonNamesStorage.LoadFromSourceCode();

#if DEBUG
            //_logger.Log($"After context.CommonNamesStorage.LoadFromSourceCode();");
#endif

            context.Storage.LoadFromSourceCode();

#if DEBUG
            //_logger.Log($"After context.Storage.LoadFromSourceCode();");
#endif

            context.StandardLibraryLoader.LoadFromSourceCode();

#if DEBUG
            //_logger.Log($"After context.StandardLibraryLoader.LoadFromSourceCode();");
#endif

            context.StatesStorage.LoadFromSourceCode();

#if DEBUG
            //_logger.Log($"After context.StatesStorage.LoadFromSourceCode();");
#endif

            context.InstancesStorage.LoadFromSourceFiles();

            result.EngineContext = context;

            return result;
        }
    }
}
