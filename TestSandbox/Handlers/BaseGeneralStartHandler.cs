using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClayDefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public abstract class BaseGeneralStartHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        protected IWorld _world;
        protected IHumanoidNPC _npc;

        protected void CreateNPC(object hostListener)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;

            //_logger.Log($"appName = {appName}");

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            //_logger.Log($"supportBasePath = {supportBasePath}");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            _world = WorldFactory.WorldInstance;

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.SharedModulesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "SymOntoClay", appName);

            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { /*ConsoleLogger.Instance,*/ CommonNLogLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true
            };

            //_logger.Log($"settings = {settings}");

            _world.SetSettings(settings);

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PeaceKeeper\PeaceKeeper.sobj");
            npcSettings.HostListener = hostListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            //_logger.Log($"npcSettings = {npcSettings}");

            _npc = _world.GetHumanoidNPC(npcSettings);
        }
    }
}
