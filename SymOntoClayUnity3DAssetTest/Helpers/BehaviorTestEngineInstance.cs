/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.World;
using SymOntoClayDefaultCLIEnvironment;
using SymOntoClayProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SymOntoClay.UnityAsset.Core.Tests.Helpers
{
    public class BehaviorTestEngineInstance: IDisposable
    {
        static BehaviorTestEngineInstance()
        {
            _rootDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), $"TstTempProjects_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_rootDir))
            {
                Directory.CreateDirectory(_rootDir);
            }
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
            _testDir = Path.Combine(rootDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_testDir))
            {
                Directory.CreateDirectory(_testDir);
            }

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = _projectName };

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, _testDir
                    , errorMsg => throw new Exception(errorMsg)
                    );

            _wSpaceDir = wSpaceFile.DirectoryName;
        }

        public void WriteFile(string fileContent)
        {
            WriteFile(_defaultRelativeFileName, fileContent);
        }

        public void WriteFile(string relativeFileName, string fileContent)
        {
            if (relativeFileName.StartsWith("/") || relativeFileName.StartsWith("\\"))
            {
                relativeFileName = relativeFileName.Substring(1);
            }

            var targetFileName = Path.Combine(_wSpaceDir, relativeFileName);

            File.WriteAllText(targetFileName, fileContent);
        }

        public static bool Run(string fileContent, Action<int, string> logChannel, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            var n = 0;

            return Run(fileContent,
                message => { n++; logChannel(n, message); },
                error => { throw new Exception(error); }
                );
        }

        public static bool Run(string fileContent, Action<string> logChannel, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            return Run(fileContent,
                message => { logChannel(message); },
                error => { throw new Exception(error); }
                );
        }

        public static bool Run(string fileContent, Action<string> logChannel, Action<string> error, int timeoutToEnd = DefaultTimeoutToEnd)
        {
            if(string.IsNullOrWhiteSpace(fileContent))
            {
                throw new Exception("Argument 'fileContent' can not be null or empty!");
            }

            using (var behaviorTestEngineInstance = new BehaviorTestEngineInstance())
            {
                behaviorTestEngineInstance.WriteFile(fileContent);

                return behaviorTestEngineInstance.Run(timeoutToEnd,
                    message => { logChannel(message); },
                    errorMsg => { error(errorMsg); }
                    );
            }
        }

        public bool Run(int timeoutToEnd, Action<string> logChannel, Action<string> error)
        {
            var result = true;

            var supportBasePath = Path.Combine(_testDir, "SysDirs");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

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
                errorMsg => { result = false; error(errorMsg); }
                );

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { callBackLogger },
                Enable = true,
                EnableRemoteConnection = true
            };

            instance.SetSettings(settings);

            var platformListener = new object();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = Path.Combine(_wSpaceDir, $"Npcs/{_projectName}/{_projectName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            Thread.Sleep(timeoutToEnd);

            return result;
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
