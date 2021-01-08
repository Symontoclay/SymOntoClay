using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstWord: TstMonoBehaviour
    {
        private readonly IEntityLogger _logger = new LoggerImpementation();

        private IWorld _world;

        public override void Awake()
        {
            _logger.Log("Begin");

            var appName = AppDomain.CurrentDomain.FriendlyName;

            _logger.Log($"appName = {appName}");

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            _logger.Log($"supportBasePath = {supportBasePath}");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = TstInvokerInMainThreadFactory.Create();

            _world = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.SharedModulesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "SymOntoClay", appName);

            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { ConsoleLogger.Instance, CommonNLogLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true
            };

            _logger.Log($"settings = {settings}");

            _world.SetSettings(settings);

            _logger.Log("End");
        }

        public override void Start()
        {
            _logger.Log("Begin");

            _world.Start();

            _logger.Log("End");
        }

        public override void Stop()
        {
            _logger.Log("Begin");

            _world.Dispose();

            _logger.Log("End");
        }
    }
}
