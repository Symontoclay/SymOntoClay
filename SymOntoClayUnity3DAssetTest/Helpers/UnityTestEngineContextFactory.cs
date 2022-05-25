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
            var testDir = CreateTestDir(CreateRootDir());

            var worldSettings = CreateWorldSettings(testDir, string.Empty, new EmptyLogger());

            var worldContext = new WorldContext();
            worldContext.SetSettings(worldSettings);



            //var npc = CreateHumanoidNPC(world, string.Empty, DefaultPlatformListener, DefaultCurrentAbsolutePosition);

            //return new ComplexTestEngineContext();

            throw new NotImplementedException();
        }
    }
}
