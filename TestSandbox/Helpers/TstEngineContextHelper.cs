/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.Logging;
using SymOntoClay.DefaultCLIEnvironment;
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
            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var appName = AppDomain.CurrentDomain.FriendlyName;

            _logger.Log($"appName = {appName}");

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            _logger.Log($"supportBasePath = {supportBasePath}");

            var result = new TstComplexContext();

            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "NpcLogs");

            var worldSettings = new WorldSettings();
            worldSettings.EnableAutoloadingConvertors = true;

            worldSettings.SharedModulesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            worldSettings.ImagesRootDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            worldSettings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "SymOntoClay", appName);

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

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PeaceKeeper\PeaceKeeper.sobj");
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Log($"npcSettings = {npcSettings}");

            var entityLogger = new LoggerImpementation();

            var tmpDir = Path.Combine(worldSettings.TmpDir, npcSettings.Id);

            Directory.CreateDirectory(worldSettings.TmpDir);

            //var standaloneStorageSettings = new StandaloneStorageSettings();
            //standaloneStorageSettings.Id = npcSettings.Id;
            //standaloneStorageSettings.IsWorld = false;
            //standaloneStorageSettings.AppFile = npcSettings.HostFile;
            //standaloneStorageSettings.Logger = entityLogger;
            //standaloneStorageSettings.Dictionary = worldContext.SharedDictionary.Dictionary;
            //standaloneStorageSettings.ModulesStorage = worldContext.ModulesStorage.ModulesStorage;
            //standaloneStorageSettings.ParentStorage = worldContext.StandaloneStorage.StandaloneStorage;

#if DEBUG
            //_logger.Log($"standaloneStorageSettings = {standaloneStorageSettings}");
#endif
            //var _hostStorage = new StandaloneStorage(standaloneStorageSettings);

            var coreEngineSettings = new EngineSettings();
            coreEngineSettings.Id = npcSettings.Id;
            coreEngineSettings.AppFile = npcSettings.LogicFile;
            coreEngineSettings.Logger = entityLogger;
            coreEngineSettings.SyncContext = worldContext.ThreadsComponent;
            
            coreEngineSettings.ModulesStorage = worldContext.ModulesStorage.ModulesStorage;
            //coreEngineSettings.ParentStorage = _hostStorage;
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
