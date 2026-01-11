/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.BaseTestLib;
using SymOntoClay.Core;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.ProjectFiles;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace TestSandbox.CreatingExamples
{
    public abstract class BaseCreatorExamples: IDisposable
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        protected BaseCreatorExamples()
            : this(new BaseCreatorExamplesOptions())
        {
        }

        protected BaseCreatorExamples(BaseCreatorExamplesOptions options)
        {
            _logger.Info("66E944F6-3C0A-43C1-A70F-B5B9F238E045", $"options = {options}");

            if(string.IsNullOrWhiteSpace(options?.DestDir))
            {
                _destDir = Path.Combine(Directory.GetCurrentDirectory(), "DestDirOfCreatorExamples");
            }

            _logger.Info("C08919DC-2DDB-457B-81DA-2BBA0D6DB617", $"_destDir = {_destDir}");

            if (Directory.Exists(_destDir))
            {
                Directory.Delete(_destDir, true);
            }

            Directory.CreateDirectory(_destDir);

            _rootDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), $"TstTempProjects_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_rootDir))
            {
                Directory.CreateDirectory(_rootDir);
            }

            _reportFileName = Path.Combine(_destDir, "report.txt");

            _logger.Info("06B092E7-822C-4032-8F73-FF0C5DA26707", $"_reportFileName = {_reportFileName}");

            if (File.Exists(_reportFileName))
            {
                File.Delete(_reportFileName);
            }

            WriteLineToReport(DateTime.Now.ToString());
        }

        private string _destDir;
        private static string _rootDir;
        private string _defaultRelativeFileName = @"/Npcs/Example/Example.soc";
        private readonly string _projectName = "Example";
        private readonly int _timeoutToEnd = 5000;
        private readonly string _reportFileName;
        private readonly string _baseRelativeExampleHref = "/docs/lng_examples/";

        protected void Example(string fileName, string fileContent)
        {
            if(string.IsNullOrWhiteSpace(fileContent))
            {
                throw new ArgumentNullException(nameof(fileContent));
            }

            using var cancellationTokenSource = new CancellationTokenSource();

            _logger.Info("4D406E02-4CAD-48FB-823F-9A8EB99A6693", $"fileContent = {fileContent}");
            _logger.Info("47AFB0CF-AC13-4863-BFD0-1249DAE87B5D", $"fileName = {fileName}");

            var targetDirectoryName = string.Empty;

            if (fileName.EndsWith(".zip"))
            {
                targetDirectoryName = fileName.Replace(".zip", string.Empty);
            }
            else
            {
                targetDirectoryName = fileName;
                fileName = $"{fileName}.zip";
            }

            WriteLineToReport("---------------------------------------------------------------------------------");
            WriteLineToReport(targetDirectoryName);
            WriteLineToReport(" ");

            _logger.Info("5712120A-42AF-4EBA-A555-7559411D42FD", $"fileName (after) = {fileName}");
            _logger.Info("A2CF00FC-05C4-4C6F-83C3-14097A57C609", $"targetDirectoryName = {targetDirectoryName}");

            var fullDestDirName = Path.Combine(_destDir, targetDirectoryName);
            var fullFileName = Path.Combine(_destDir, fileName); 

            _logger.Info("A8707CC1-5A37-46E8-8E50-9C8F8A915494", $"fullDestDirName = {fullDestDirName}");
            _logger.Info("08A54688-AA19-4FB5-841B-932596B2752B", $"fullFileName = {fullFileName}");

            var relativeExampleHref = $"{_baseRelativeExampleHref}{fileName}";

            _logger.Info("CC8BCD94-F831-4B81-A535-7962CE94BCBB", $"relativeExampleHref = {relativeExampleHref}");

            WriteLineToReport($"<code data-lng='soc' example-href='{relativeExampleHref}'>");
            WriteLineToReport(fileContent);
            WriteLineToReport("</code>");
            WriteLineToReport(" ");

            var testDir = Path.Combine(_rootDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = _projectName };

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, testDir
                    , errorMsg => throw new Exception(errorMsg)
                    );

            var wSpaceDir = wSpaceFile.DirectoryName;

            _logger.Info("CC12B555-6871-4743-9E20-26602F7B695A", $"testDir = {testDir}");
            _logger.Info("1F06C648-9F72-4662-9397-803FCB8E2E4B", $"wSpaceDir = {wSpaceDir}");

            var relativeFileName = _defaultRelativeFileName;

            if (relativeFileName.StartsWith("/") || relativeFileName.StartsWith("\\"))
            {
                relativeFileName = relativeFileName.Substring(1);
            }

            var targetFileName = Path.Combine(wSpaceDir, relativeFileName);

            File.WriteAllText(targetFileName, fileContent);

            var supportBasePath = Path.Combine(testDir, "SysDirs");

            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create(cancellationTokenSource.Token);

            var instance = new WorldCore();

            var settings = new WorldSettings();
            settings.CancellationToken = cancellationTokenSource.Token;
            settings.EnableAutoloadingConvertors = true;

            settings.LibsDirs = new List<string>() { Path.Combine(wSpaceDir, "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(supportBasePath, "TMP");

            settings.HostFile = Path.Combine(wSpaceDir, "World/World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            var callBackLogger = new CallBackLogger(
                message => 
                {
                    WriteLineToReport(NormalizeTextForConsole(message));
                    _logger.Info("9049A34F-61F8-4A35-A591-AC24397CEBFA", message);
                },
                errorMsg => {
                    _logger.Error("0925690E-24A6-4D53-92BE-1387DF43D65A", errorMsg);
                }
                );

            settings.Monitor = new SymOntoClay.Monitor.Monitor(new SymOntoClay.Monitor.MonitorSettings
            {
                MessagesDir = monitorMessagesDir,
                PlatformLoggers = new List<IPlatformLogger>() { callBackLogger },
                Enable = true,
                CancellationToken = cancellationTokenSource.Token,
                ThreadingSettings = ThreadingSettingsHepler.ConfigureMonitorThreadingSettings()
            });

            ThreadingSettingsHepler.ConfigureThreadingSettings(settings);

            instance.SetSettings(settings);

            var platformListener = new object();

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = Path.Combine(wSpaceDir, $"Npcs/{_projectName}/{_projectName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();
            npcSettings.ThreadingSettings = ThreadingSettingsHepler.ConfigureHumanoidNpcDefaultThreadingSettings();

            var npc = instance.GetHumanoidNPC(npcSettings);

            WriteLineToReport("<console>");

            instance.Start();

            Thread.Sleep(_timeoutToEnd);

            WriteLineToReport("</console>");

            Directory.Move(wSpaceDir, fullDestDirName);

            ZipFile.CreateFromDirectory(fullDestDirName, fullFileName);

            Directory.Delete(testDir, true);
            Directory.Delete(fullDestDirName, true);

            cancellationTokenSource.Cancel();
        }

        protected string CreateName(string prefix, int n)
        {
            return $"{prefix}_Example{n}";
        }

        private string NormalizeTextForConsole(string source)
        {
            return source.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private void WriteLineToReport(string text)
        {
            File.AppendAllLines(_reportFileName, new List<string>() { text });
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

        }
    }
}
