using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
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
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
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

        public static WorldSettings CreateWorldSettings(string baseDir, string hostFile, IPlatformLogger platformLogger)
        {
            var supportBasePath = Path.Combine(baseDir, "SysDirs");

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

        public static IWorld CreateWorld(string baseDir, string hostFile, IPlatformLogger platformLogger)
        {
            var settings = CreateWorldSettings(baseDir, hostFile, platformLogger);

            return CreateWorld(settings);
        }

        public static IWorld CreateWorld(WorldSettings settings)
        {
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

            return CreateHumanoidNPC(world, npcSettings);
        }

        public static IHumanoidNPC CreateHumanoidNPC(IWorld world, HumanoidNPCSettings npcSettings)
        {
            var npc = world.GetHumanoidNPC(npcSettings);

            ILoggedTestHostListener loggedTestHostListener = null;

            var platformListener = npcSettings.HostListener;

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

            var baseDir = CreateTestDir(CreateRootDir());

            var worldSettings = CreateWorldSettings(baseDir, string.Empty, entityLogger);

            var npcSettings = CreateHumanoidNPCSettings(string.Empty, DefaultPlatformListener, DefaultCurrentAbsolutePosition);

            return CreateTestEngineContext(worldSettings, npcSettings);
        }

        public static ComplexTestEngineContext CreateTestEngineContext(WorldSettings worldSettings, HumanoidNPCSettings npcSettings)
        {
            return CreateTestEngineContext(worldSettings, npcSettings, string.Empty);
        }

        public static ComplexTestEngineContext CreateTestEngineContext(WorldSettings worldSettings, HumanoidNPCSettings npcSettings, string baseDir)
        {
            var world = CreateWorld(worldSettings);

            var npc = CreateHumanoidNPC(world, npcSettings);

            return new ComplexTestEngineContext(world, npc, baseDir);
        }

        public static INLPConverterContext CreateNLPConverterContext(IEngineContext engineContext)
        {
            var dataResolversFactory = engineContext.DataResolversFactory;

            var relationsResolver = dataResolversFactory.GetRelationsResolver();
            var inheritanceResolver = dataResolversFactory.GetInheritanceResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = engineContext.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = NameHelper.CreateName(engineContext.Id);

            var packedRelationsResolver = new PackedRelationsResolver(relationsResolver, localCodeExecutionContext);

            var packedInheritanceResolver = new PackedInheritanceResolver(inheritanceResolver, localCodeExecutionContext);

            return new NLPConverterContext(packedRelationsResolver, packedInheritanceResolver);
        }
    }
}
