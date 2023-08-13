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

using NLog;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using SymOntoClay.SoundBuses;
using System.Threading.Tasks;
using System.Configuration;
using SymOntoClay.CoreHelper;
using Newtonsoft.Json;
using System.Diagnostics;
using SymOntoClay.CoreHelper.DebugHelpers;
using System.Linq;
using SymOntoClay.ProjectFiles;
using SymOntoClay.NLP;

namespace SymOntoClay.CLI
{
    public class CLIRunHandler : IDisposable
    {
        private IWorld world;

        public void Run(CLICommand command)
        {
            if(!command.NoLogo)
            {
                ConsoleWrapper.WriteText($"Loading {command.InputFile}...");
            }

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = command.InputDir,
                InputFile = command.InputFile,
                AppName = @"SymOntoClay\CLI"
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;
            world = instance;

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.LibsDirs = new List<string>() { targetFiles.SharedLibsDir, targetFiles.LibsDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            if(command.UseNLP)
            {
                var nlpConverterProviderSettings = new NLPConverterProviderSettings();

                var socAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name == "soc").Location;

                var assemblyPath = (new FileInfo(socAssembly)).DirectoryName;

                var mainDictPath = Path.Combine(assemblyPath, "Dicts", "BigMainDictionary.dict");

                nlpConverterProviderSettings.DictsPaths = new List<string>() { mainDictPath };

                nlpConverterProviderSettings.CreationStrategy = CreationStrategy.Singleton;

                var nlpConverterProvider = new NLPConverterProvider(nlpConverterProviderSettings);

                settings.NLPConverterProvider = nlpConverterProvider;
            }

            var logDir = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", "CLI", "NpcLogs");

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                PlatformLoggers = new List<IPlatformLogger>() { new CLIPlatformLogger() },
                Enable = true,
                EnableRemoteConnection = true
            };

            instance.SetSettings(settings);

            var platformListener = this;

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            _npcLogger = npc.Logger;

            if (command.Timeout.HasValue)
            {
                var timeoutValue = command.Timeout.Value;

                Task.Run(() => {
                    try
                    {
                        if (timeoutValue > 0)
                        {
                            Thread.Sleep(timeoutValue);
                        }

                        Environment.Exit(0);
                    }
                    catch(Exception e)
                    {
                        _npcLogger.Error(e.ToString());
                    }
                });
            }
            else
            {
                ConsoleWrapper.WriteText("Press 'exit' and Enter for exit.");
            }            

            while(true)
            {
                var inputStr = Console.ReadLine().Trim();

                ConsoleWrapper.WriteText(inputStr);

                if (inputStr == "exit")
                {
                    ConsoleWrapper.WriteText("Are you sure? Type y/n and press Enter.");

                    inputStr = Console.ReadLine();

                    if(inputStr == "y")
                    {
                        break;
                    }
                }
                else
                {
                    ConsoleWrapper.WriteError("Unknown command! Press 'exit' and Enter for exit.");
                }
            }
        }

        private IEntityLogger _npcLogger;

        public void Dispose()
        {
            world?.Dispose();
        }

        [DebuggerHidden]
        [BipedEndpoint("*")]
        public void GenericCall(CancellationToken cancellationToken, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            _npcLogger.Log($"methodName = '{methodName}'");
            _npcLogger.Log($"isNamedParameters = {isNamedParameters}");
            _npcLogger.Log($"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented)}");
            _npcLogger.Log($"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented)}");
        }
    }
}
