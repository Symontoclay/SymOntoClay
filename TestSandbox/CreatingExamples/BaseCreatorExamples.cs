/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.World;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.ProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;
using SymOntoClay.BaseTestLib;

namespace TestSandbox.CreatingExamples
{
    public abstract class BaseCreatorExamples: IDisposable
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        protected BaseCreatorExamples()
            : this(new BaseCreatorExamplesOptions())
        {
        }

        protected BaseCreatorExamples(BaseCreatorExamplesOptions options)
        {
            _logger.Log($"options = {options}");

            if(string.IsNullOrWhiteSpace(options?.DestDir))
            {
                _destDir = Path.Combine(Directory.GetCurrentDirectory(), "DestDirOfCreatorExamples");
            }

            _logger.Log($"_destDir = {_destDir}");

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

            _logger.Log($"_reportFileName = {_reportFileName}");

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

            _logger.Log($"fileContent = {fileContent}");
            _logger.Log($"fileName = {fileName}");

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

            _logger.Log($"fileName (after) = {fileName}");
            _logger.Log($"targetDirectoryName = {targetDirectoryName}");

            var fullDestDirName = Path.Combine(_destDir, targetDirectoryName);
            var fullFileName = Path.Combine(_destDir, fileName); 

            _logger.Log($"fullDestDirName = {fullDestDirName}");
            _logger.Log($"fullFileName = {fullFileName}");

            var relativeExampleHref = $"{_baseRelativeExampleHref}{fileName}";

            _logger.Log($"relativeExampleHref = {relativeExampleHref}");

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

            _logger.Log($"testDir = {testDir}");
            _logger.Log($"wSpaceDir = {wSpaceDir}");

            var relativeFileName = _defaultRelativeFileName;

            if (relativeFileName.StartsWith("/") || relativeFileName.StartsWith("\\"))
            {
                relativeFileName = relativeFileName.Substring(1);
            }

            var targetFileName = Path.Combine(wSpaceDir, relativeFileName);

            File.WriteAllText(targetFileName, fileContent);

            var supportBasePath = Path.Combine(testDir, "SysDirs");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var instance = new WorldCore();

            var settings = new WorldSettings();
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
                    _logger.Log(message);
                },
                errorMsg => {
                    _logger.Error(errorMsg);
                }
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
            npcSettings.LogicFile = Path.Combine(wSpaceDir, $"Npcs/{_projectName}/{_projectName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            var npc = instance.GetHumanoidNPC(npcSettings);

            WriteLineToReport("<console>");

            instance.Start();

            Thread.Sleep(_timeoutToEnd);

            WriteLineToReport("</console>");

            Directory.Move(wSpaceDir, fullDestDirName);

            ZipFile.CreateFromDirectory(fullDestDirName, fullFileName);

            Directory.Delete(testDir, true);
            Directory.Delete(fullDestDirName, true);
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

            //Directory.Delete(_rootDir, true);
        }
    }
}
