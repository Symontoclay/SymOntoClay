/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.CLI.Helpers;
using SymOntoClay.Core;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.Monitor.Common;
using SymOntoClay.NLP;
using SymOntoClay.ProjectFiles;
using SymOntoClay.SoundBuses;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace SymOntoClay.CLI
{
    public class CLIRunHandler : IDisposable
    {
        private IWorld world;

        private CancellationTokenSource _cancellationTokenSource;

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

            _cancellationTokenSource = new CancellationTokenSource();

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create(_cancellationTokenSource.Token);

            var instance = WorldFactory.WorldInstance;
            world = instance;

            var settings = new WorldSettings();

            settings.CancellationToken = _cancellationTokenSource.Token;

            settings.EnableAutoloadingConvertors = true;

            settings.LibsDirs = new List<string>() { targetFiles.SharedLibsDir, targetFiles.LibsDir };

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            settings.HostFile = targetFiles.WorldFile;

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus(new SimpleSoundBusSettings
            {
                CancellationToken = _cancellationTokenSource.Token,
                ThreadingSettings = ConfigureSoundBusThreadingSettings()
            });

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

            var monitorMessagesDir = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", "CLI", "NpcMonitorMessages");

            settings.Monitor = new SymOntoClay.Monitor.Monitor(new Monitor.MonitorSettings()
            {
                MessagesDir = monitorMessagesDir,
                PlatformLoggers = new List<IPlatformLogger>() { new CLIPlatformLogger() },
                Enable = true
            });

            settings.WorldThreadingSettings = ConfigureWorldThreadingSettings();

            var humanoidNpcThreadingSettings = ConfigureHumanoidNpcThreadingSettings();

            settings.HumanoidNpcDefaultThreadingSettings = humanoidNpcThreadingSettings;
            settings.PlayerDefaultThreadingSettings = ConfigurePlayerDefaultThreadingSettings();
            settings.GameObjectDefaultThreadingSettings = ConfigureGameObjectDefaultThreadingSettings();
            settings.PlaceDefaultThreadingSettings = ConfigurePlaceDefaultThreadingSettings();

            instance.SetSettings(settings);

            var platformListener = this;

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.LogicFile = targetFiles.LogicFile;
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            npcSettings.ThreadingSettings = humanoidNpcThreadingSettings;

            var npc = instance.GetHumanoidNPC(npcSettings);

            instance.Start();

            _npcLogger = npc.Logger;

            if (command.Timeout.HasValue)
            {
                var timeoutValue = command.Timeout.Value;

                ThreadTask.Run(() => {
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
                        _npcLogger.Error("36F19773-BB0C-4216-A713-31CD3502BED9", e);
                    }
                }, _cancellationTokenSource.Token);
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

        private IMonitorLogger _npcLogger;

        private ThreadingSettings ConfigureWorldThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 10,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 10,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        private ThreadingSettings ConfigureHumanoidNpcThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 50
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 50
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                }
            };
        }

        private ThreadingSettings ConfigurePlayerDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        private ThreadingSettings ConfigureGameObjectDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        private ThreadingSettings ConfigurePlaceDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        private CustomThreadPoolSettings ConfigureSoundBusThreadingSettings()
        {
            return new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 1
            };
        }

        private CustomThreadPoolSettings ConfigureMonitorThreadingSettings()
        {
            return new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 10
            };
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            world?.Dispose();
            _cancellationTokenSource.Dispose();
        }

        [DebuggerHidden]
        [BipedEndpoint("*")]
        public void GenericCall(CancellationToken cancellationToken, string methodName, bool isNamedParameters,
            Dictionary<string, object> namedParameters, List<object> positionedParameters)
        {
            _npcLogger.Info("3478400A-0265-48B9-BF32-7B1604251CB3", $"methodName = '{methodName}'");
            _npcLogger.Info("5FAD50B6-A6A7-49F1-856F-0F088CF017EE", $"isNamedParameters = {isNamedParameters}");
            _npcLogger.Info("91117387-A87E-4392-82FB-BC2AB8343DFA", $"namedParameters = {JsonConvert.SerializeObject(namedParameters, Formatting.Indented)}");
            _npcLogger.Info("422B550B-195E-4F52-9C89-9BCE68FA10AF", $"positionedParameters = {JsonConvert.SerializeObject(positionedParameters, Formatting.Indented)}");
        }
    }
}
