using NLog;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class GeneralStartHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "NpcLogs");

            var instance = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.SourceFilesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "Modules") };

            settings.ImagesRootDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");

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

            _logger.Info($"npcSettings = {npcSettings}");

            var npc = instance.GetBipedNPC(npcSettings);

            instance.Start();

            _logger.Info("End");
        }
    }
}
