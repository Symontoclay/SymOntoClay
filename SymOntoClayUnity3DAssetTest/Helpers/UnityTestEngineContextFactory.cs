using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.SoundBuses;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.Helpers
{
    public static class UnityTestEngineContextFactory
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static string CreateRootDir()
        {
            var rootDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), $"TstTempProjects_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }

            return rootDir;
        }

        public static string CreateTestDir(string rootDir)
        {
            var testDir = Path.Combine(rootDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            return testDir;
        }

        public static WorldSettings CreateWorldSettings(string testDir, string hostFile, IPlatformLogger platformLogger)
        {
            var supportBasePath = Path.Combine(testDir, "SysDirs");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.BuiltInStandardLibraryDir = DefaultPaths.GetBuiltInStandardLibraryDir();

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(supportBasePath, "TMP");

            if (!string.IsNullOrWhiteSpace(hostFile))
            {
                settings.HostFile = hostFile;
            }

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { platformLogger },
                Enable = true,
                EnableRemoteConnection = true
            };

            return settings;
        }

        public static IWorld CreateWorld(string testDir, string hostFile, IPlatformLogger platformLogger)
        {
            var settings = CreateWorldSettings(testDir, hostFile, platformLogger);

            var world = new WorldCore();
            world.SetSettings(settings);

            return world;
        }

        private static object _lockObj = new object();
        private static int _currInstanceId;

        public static int GetInstanceId()
        {
            lock (_lockObj)
            {
                _currInstanceId++;
                return _currInstanceId;
            }
        }

        public static object DefaultPlatformListener => new object();
        public static Vector3 DefaultCurrentAbsolutePosition => new Vector3(10, 10, 10);

        public static HumanoidNPCSettings CreateHumanoidNPCSettings(string logicFile, object platformListener, Vector3 currentAbsolutePosition)
        {
            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = $"#{Guid.NewGuid():D}";
            npcSettings.InstanceId = GetInstanceId();

            if(!string.IsNullOrWhiteSpace(logicFile))
            {
                npcSettings.LogicFile = logicFile;
            }
            
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub(currentAbsolutePosition);

            return npcSettings;
        }

        public static IHumanoidNPC CreateHumanoidNPC(IWorld world, string logicFile, object platformListener, Vector3 currentAbsolutePosition)
        {
            var npcSettings = CreateHumanoidNPCSettings(logicFile, platformListener, currentAbsolutePosition);

            var npc = world.GetHumanoidNPC(npcSettings);

            ILoggedTestHostListener loggedTestHostListener = null;

            if (platformListener != null)
            {
                loggedTestHostListener = platformListener as ILoggedTestHostListener;
            }

            loggedTestHostListener?.SetLogger(npc.Logger);

            return npc;
        }

        public static ComplexTestEngineContext CreateTestEngineContext()
        {
            var entityLogger = new EmptyLogger();

            var testDir = CreateTestDir(CreateRootDir());

            var worldSettings = CreateWorldSettings(testDir, string.Empty, entityLogger);

#if DEBUG
            //_gbcLogger.Info($"worldSettings = {worldSettings}");
#endif

            var worldContext = new WorldContext();
            worldContext.SetSettings(worldSettings);

            var npcSettings = CreateHumanoidNPCSettings(string.Empty, DefaultPlatformListener, DefaultCurrentAbsolutePosition);

#if DEBUG
            //_gbcLogger.Info($"npcSettings = {npcSettings}");
#endif

            var worldCoreGameComponentContext = worldContext as IWorldCoreGameComponentContext;

            var coreEngineSettings = new EngineSettings();
            coreEngineSettings.Id = npcSettings.Id;
            coreEngineSettings.AppFile = npcSettings.LogicFile;
            coreEngineSettings.Logger = entityLogger;
            coreEngineSettings.SyncContext = worldContext.ThreadsComponent;

            coreEngineSettings.ModulesStorage = worldContext.ModulesStorage.ModulesStorage;
            coreEngineSettings.ParentStorage = worldCoreGameComponentContext.StandaloneStorage;
            //coreEngineSettings.ParentStorage = _hostStorage;
            coreEngineSettings.TmpDir = testDir;
            coreEngineSettings.HostSupport = new HostSupportComponentStub(npcSettings.PlatformSupport);

#if DEBUG
            //_gbcLogger.Info($"coreEngineSettings = {coreEngineSettings}");
#endif

            var engineContext = EngineContextHelper.CreateAndInitContext(coreEngineSettings);

#if DEBUG
            //_gbcLogger.Info($"After var engineContext = EngineContextHelper.CreateAndInitContext(coreEngineSettings);");
#endif

            engineContext.CommonNamesStorage.LoadFromSourceCode();

#if DEBUG
            //_gbcLogger.Info($"After engineContext.CommonNamesStorage.LoadFromSourceCode();");
#endif

            engineContext.Storage.LoadFromSourceCode();

#if DEBUG
            //_gbcLogger.Info($"After engineContext.Storage.LoadFromSourceCode();");
#endif

            engineContext.StandardLibraryLoader.LoadFromSourceCode();

#if DEBUG
            //_gbcLogger.Info($"After engineContext.StandardLibraryLoader.LoadFromSourceCode();");
#endif

            engineContext.InstancesStorage.LoadFromSourceFiles();

            return new ComplexTestEngineContext(worldContext, engineContext, testDir);
        }
    }
}
