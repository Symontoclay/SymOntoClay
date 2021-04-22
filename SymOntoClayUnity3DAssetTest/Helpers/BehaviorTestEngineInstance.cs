using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.World;
using SymOntoClayDefaultCLIEnvironment;
using SymOntoClayProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SymOntoClay.Unity3DAsset.Test.Helpers
{
    public class BehaviorTestEngineInstance: IDisposable
    {
#if DEBUG
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        static BehaviorTestEngineInstance()
        {
            _rootDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), $"TstTempProjects_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_rootDir))
            {
                Directory.CreateDirectory(_rootDir);
            }

#if DEBUG
            _logger.Info($"Construct _rootDir = {_rootDir}");
#endif
        }

        private static string _rootDir;

        public static string RoorDir => _rootDir;

        public const int DefaultTimeoutToEnd = 5000;

        public BehaviorTestEngineInstance()
            : this(RoorDir)
        {
        }

        public BehaviorTestEngineInstance(string rootDir)
        {
#if DEBUG
            _logger.Info($"rootDir = {rootDir}");
#endif

            _testDir = Path.Combine(rootDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

#if DEBUG
            _logger.Info($"_testDir = {_testDir}");
#endif

            if (!Directory.Exists(_testDir))
            {
                Directory.CreateDirectory(_testDir);
            }

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = _projectName };

#if DEBUG
            _logger.Info($"worldSpaceCreationSettings = {worldSpaceCreationSettings}");
#endif

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, _testDir
                    , errorMsg => throw new Exception(errorMsg)
                    );

            _logger.Info($"wSpaceFile = {wSpaceFile}");

            _wSpaceDir = wSpaceFile.DirectoryName;

            _logger.Info($"_wSpaceDir = {_wSpaceDir}");
        }

        public void WriteFile(string fileContent)
        {
#if DEBUG
            _logger.Info($"fileContent = {fileContent}");
#endif

            WriteFile(_defaultRelativeFileName, fileContent);
        }

        public void WriteFile(string relativeFileName, string fileContent)
        {
#if DEBUG
            _logger.Info($"relativeFileName = {relativeFileName}");
            _logger.Info($"fileContent = {fileContent}");
#endif

            if (relativeFileName.StartsWith("/") || relativeFileName.StartsWith("\\"))
            {
                relativeFileName = relativeFileName.Substring(1);
            }

#if DEBUG
            _logger.Info($"targetRelativeFileName (after) = {relativeFileName}");
#endif
            var targetFileName = Path.Combine(_wSpaceDir, relativeFileName);

#if DEBUG
            _logger.Info($"targetFileName = {targetFileName}");
#endif

            File.WriteAllText(targetFileName, fileContent);
        }

        public static void Run(string fileContent, Action<string> logChannel, Action<string> error, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            var behaviorTestEngineInstance = new BehaviorTestEngineInstance();

            behaviorTestEngineInstance.WriteFile(fileContent);

            behaviorTestEngineInstance.Run(timeoutToEnd,
                message => { _logger.Log($"message = {message}"); },
                error => { _logger.Log($"error = {error}"); }
                );
        }

        public void Run(int timeoutToEnd, Action<string> logChannel, Action<string> error)
        {
#if DEBUG
            _logger.Info($"timeoutToEnd = {timeoutToEnd}");
#endif

            var supportBasePath = Path.Combine(_testDir, "SysDirs");

#if DEBUG
            _logger.Info($"supportBasePath = {supportBasePath}");
#endif
            var logDir = Path.Combine(supportBasePath, "NpcLogs");

#if DEBUG
            _logger.Info($"logDir = {logDir}");
#endif
            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var instance = new WorldCore();

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.SharedModulesDirs = new List<string>() { Path.Combine(_wSpaceDir, "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(supportBasePath, "TMP");

            settings.HostFile = Path.Combine(_wSpaceDir, "World/World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            var callBackLogger = new CallBackLogger(
                message => { logChannel(message); },
                errorMsg => { error(errorMsg); }
                );

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { callBackLogger },
                Enable = true,
                EnableRemoteConnection = true
            };

#if DEBUG
            _logger.Info($"settings = {settings}");
#endif
            instance.SetSettings(settings);

            var platformListener = new object();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = Path.Combine(_wSpaceDir, $"Npcs/{_projectName}/{_projectName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

#if DEBUG
            _logger.Info($"npcSettings = {npcSettings}");
#endif
            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(timeoutToEnd);
        }

        private string _defaultRelativeFileName = @"/Npcs/Example/Example.soc";
        private readonly string _projectName = "Example";
        private readonly string _testDir;
        private readonly string _wSpaceDir;

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            Directory.Delete(_testDir, true);
        }
    }
}
