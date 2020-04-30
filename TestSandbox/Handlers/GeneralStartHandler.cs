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

            var instance = WorldCore.Instance;

            var settings = new WorldSettings();
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

            _logger.Info($"npcSettings = {npcSettings}");

            var npc = instance.GetBipedNPC(npcSettings);

            _logger.Info("End");
        }
    }
}
