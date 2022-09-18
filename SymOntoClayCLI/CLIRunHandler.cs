/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

namespace SymOntoClay.CLI
{
    public class CLIRunHandler : IDisposable
    {
#if DEBUG
        //private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        private IWorld world;

        public void Run(CLICommand command)
        {
            if(!command.NoLogo)
            {
                ConsoleWrapper.WriteText($"Loading {command.InputFile}...");
            }

#if DEBUG
            //_logger.Info($"command = {command}");
#endif

            var appSettings = ConfigurationManager.AppSettings;

            var targetFiles = RunCommandFilesSearcher.Run(command);

#if DEBUG
            //_logger.Info($"targetFiles = {targetFiles}");
#endif

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;
            world = instance;

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.SharedModulesDirs = new List<string>() { targetFiles.SharedModulesDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            var standardLibraryBasePath = appSettings["StandardLibraryPath"];

            settings.BuiltInStandardLibraryDir = EVPath.Normalize(standardLibraryBasePath);

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            var logDir = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", "CLI", "NpcLogs");

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { new CLIPlatformLogger() },
                Enable = true,
                EnableRemoteConnection = true
            };

#if DEBUG
            //_logger.Info($"settings = {settings}");
#endif
            instance.SetSettings(settings);

            var platformListener = this;

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

#if DEBUG
            //_logger.Info($"npcSettings = {npcSettings}");
#endif
            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            if(command.Timeout.HasValue)
            {
                var timeoutValue = command.Timeout.Value;

                Task.Run(() => { 
                    if(timeoutValue > 0)
                    {
                        Thread.Sleep(timeoutValue);
                    }

                    Environment.Exit(0);
                });
            }
            else
            {
                ConsoleWrapper.WriteText("Press 'exit' and Enter for exit.");
            }            

            while(true)
            {
                var inputStr = Console.ReadLine();

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

        public void Dispose()
        {
            world?.Dispose();
        }

        [BipedEndpoint("Go", DeviceOfBiped.RightLeg, DeviceOfBiped.LeftLeg)]
        public void GoToImpl(CancellationToken cancellationToken,
             [EndpointParam("To", KindOfEndpointParam.Position)] Vector3 point,
             float speed = 12)
        {
            //var name = NameHelper.GetNewEntityNameString();
            var name = string.Empty;

            //_logger.Log($"Begin {name}");

            //_logger.Log($"{name} point = {point}");
            //_logger.Log($"{name} speed = {speed}");

            var n = 0;

            while (true)
            {
                n++;

                if (n > 10)
                {
                    break;
                }

                Thread.Sleep(1000);

                //_logger.Log($"{name} Hi! n = {n}");

                cancellationToken.ThrowIfCancellationRequested();
            }

            //Thread.Sleep(5000);

            //_logger.Log($"End {name}");
        }
    }
}
