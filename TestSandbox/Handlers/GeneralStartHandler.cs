using NLog;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class GeneralStartHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "NpcLogs");

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.SourceFilesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            settings.ImagesRootDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            settings.TmpDir = Path.Combine(Directory.GetCurrentDirectory(), "Tmp");

            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\World.txt");

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { ConsoleLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true
            };

            _logger.Info($"settings = {settings}");

            instance.SetSettings(settings);

            var npcSettings = new BipedNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PixKeeper\PixKeeper.txt");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Apps\PixKeeper\PixKeeper.txt");

            _logger.Info($"npcSettings = {npcSettings}");

            var npc = instance.GetBipedNPC(npcSettings);

            instance.Start();

            Thread.Sleep(5000);

            _logger.Info("End");
        }
    }
}
