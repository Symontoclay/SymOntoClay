using NLog;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.CLI
{
    public class CLIRunHandler : IDisposable
    {
#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        private IWorld world;

        public void Run(CLICommand command)
        {
#if DEBUG
            _logger.Info($"command = {command}");
#endif

            var targetFiles = RunCommandFilesSearcher.Run(command);

#if DEBUG
            _logger.Info($"targetFiles = {targetFiles}");
#endif

            //var invokingInMainThread = TstInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;
            world = instance;

            var settings = new WorldSettings();

            settings.SharedModulesDirs = new List<string>() { targetFiles.SharedModulesDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            //settings.InvokerInMainThread = invokingInMainThread;

            settings.Logging = new LoggingSettings()
            {
                //LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { new CLIPlatformLogger() },
                Enable = true,
                EnableRemoteConnection = true
            };

#if DEBUG
            _logger.Info($"settings = {settings}");
#endif
            instance.SetSettings(settings);

            var platformListener = this;

            var npcSettings = new BipedNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            //npcSettings.PlatformSupport = new TstPlatformSupport();

#if DEBUG
            _logger.Info($"npcSettings = {npcSettings}");
#endif
            var npc = instance.GetBipedNPC(npcSettings);

            instance.Start();

            Thread.Sleep(50000);

            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
