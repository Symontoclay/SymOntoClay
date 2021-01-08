/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;

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
            ConsoleWrapper.WriteText($"Loading {command.InputFile}...");

#if DEBUG
            //_logger.Info($"command = {command}");
#endif

            var targetFiles = RunCommandFilesSearcher.Run(command);

#if DEBUG
            //_logger.Info($"targetFiles = {targetFiles}");
#endif

            //var invokingInMainThread = TstInvokerInMainThreadFactory.Create();

            var instance = WorldFactory.WorldInstance;
            world = instance;

            var settings = new WorldSettings();
            settings.AutoloadingConvertors = true;

            settings.SharedModulesDirs = new List<string>() { targetFiles.SharedModulesDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            //settings.InvokerInMainThread = invokingInMainThread;

            //var logDir = Path.Combine(Directory.GetCurrentDirectory(), "NpcLogs");

            settings.Logging = new LoggingSettings()
            {
                //LogDir = logDir,
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

            ConsoleWrapper.WriteText("Press 'exit' and Enter for exit.");

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
