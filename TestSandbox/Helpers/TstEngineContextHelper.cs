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

using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;
using SymOntoClay.SoundBuses;
using SymOntoClay.NLP;
using SymOntoClay.StandardFacts;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.ProjectFiles;
using SymOntoClay.BaseTestLib;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

namespace TestSandbox.Helpers
{
    public static class TstEngineContextHelper
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public static WorldSettings CreateWorldSettings(UnityTestEngineContextFactorySettings factorySettings)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;

            
            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);


            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var codeDir = Path.Combine(Directory.GetCurrentDirectory(), "Source");


            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = codeDir,
                AppName = $@"SymOntoClay\{appName}"
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            var libsDirs = new List<string>();

            if(!string.IsNullOrWhiteSpace(targetFiles.SharedLibsDir))
            {
                libsDirs.Add(targetFiles.SharedLibsDir);
            }

            if(!string.IsNullOrWhiteSpace(targetFiles.LibsDir))
            {
                libsDirs.Add(targetFiles.LibsDir);
            }

            settings.LibsDirs = libsDirs;

            settings.ImagesRootDir = targetFiles.ImagesRootDir;

            settings.TmpDir = targetFiles.TmpDir;

            if(string.IsNullOrWhiteSpace(factorySettings.WorldFile))
            {
                if (factorySettings.UseDefaultAppFiles)
                {
                    settings.HostFile = targetFiles.WorldFile;
                }
            }
            else
            {
                settings.HostFile = factorySettings.WorldFile;
            }

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            if(!factorySettings.DictsPaths.IsNullOrEmpty() || !factorySettings.DictsList.IsNullOrEmpty() || factorySettings.UseDefaultNLPSettings)
            {
                var nlpConverterProviderSettings = new NLPConverterProviderSettings();

                if(!factorySettings.DictsPaths.IsNullOrEmpty() || !factorySettings.DictsList.IsNullOrEmpty())
                {
                    nlpConverterProviderSettings.DictsPaths = factorySettings.DictsPaths;
                    nlpConverterProviderSettings.DictsList = factorySettings.DictsList;
                }
                else
                {
                    if(factorySettings.UseDefaultNLPSettings)
                    {
                        var mainDictPath = Path.Combine(Directory.GetCurrentDirectory(), "Dicts", "BigMainDictionary.dict");

                        nlpConverterProviderSettings.DictsPaths = new List<string>() { mainDictPath };
                    }
                }
                
                nlpConverterProviderSettings.CreationStrategy = CreationStrategy.Singleton;

                var nlpConverterProvider = new NLPConverterProvider(nlpConverterProviderSettings);

                settings.NLPConverterProvider = nlpConverterProvider;
            }

            settings.StandardFactsBuilder = new StandardFactsBuilder();

            var monitorSettings = new SymOntoClay.Monitor.MonitorSettings
            {
                Enable = true,
                MessagesDir = monitorMessagesDir,
                KindOfLogicalSearchExplain = KindOfLogicalSearchExplain.DumpAlways,
                //LogicalSearchExplainDumpDir = Directory.GetCurrentDirectory(),
                EnableAddingRemovingFactLoggingInStorages = true,
                EnableFullCallInfo = true
            };

            if(factorySettings.PlatformLogger != null)
            {
                monitorSettings.PlatformLoggers = new List<IPlatformLogger>() { factorySettings.PlatformLogger };
            }
            else
            {
                if(factorySettings.UseDefaultPlatformLogger)
                {
                    monitorSettings.PlatformLoggers = new List<IPlatformLogger>() { /*ConsoleLogger.Instance,*/ CommonNLogLogger.Instance };
                }
            }

            settings.Monitor = new SymOntoClay.Monitor.Monitor(monitorSettings);

            return settings;
        }

        public static HumanoidNPCSettings CreateHumanoidNPCSettings(UnityTestEngineContextFactorySettings factorySettings)
        {
            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.InstanceId = 1;
            
            if(string.IsNullOrWhiteSpace(factorySettings.NPCAppFile))
            {
                if (factorySettings.UseDefaultAppFiles)
                {
                    npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PeaceKeeper\PeaceKeeper.sobj");
                }
            }
            else
            {
                npcSettings.LogicFile = factorySettings.NPCAppFile;
            }

            if(factorySettings.HostListener == null)
            {
                if(factorySettings.UseDefaultHostListener)
                {
                    npcSettings.HostListener = UnityTestEngineContextFactory.DefaultPlatformListener;
                }
            }
            else
            {
                npcSettings.HostListener = factorySettings.HostListener;
            }
            
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            npcSettings.Categories = factorySettings.Categories;
            npcSettings.EnableCategories = factorySettings.EnableCategories;

            return npcSettings;
        }

        public static IWorld CreateWorld(UnityTestEngineContextFactorySettings factorySettings)
        {
            var settings = CreateWorldSettings(factorySettings);


            return UnityTestEngineContextFactory.CreateWorld(settings);
        }

        public static IHumanoidNPC CreateNPC(IWorld world, UnityTestEngineContextFactorySettings factorySettings)
        {
            var npcSettings = CreateHumanoidNPCSettings(factorySettings);


            return UnityTestEngineContextFactory.CreateHumanoidNPC(world, npcSettings);
        }

        public static ComplexTestEngineContext CreateContext(UnityTestEngineContextFactorySettings factorySettings)
        {
            var settings = CreateWorldSettings(factorySettings);


            var npcSettings = CreateHumanoidNPCSettings(factorySettings);


            return UnityTestEngineContextFactory.CreateTestEngineContext(settings, npcSettings);
        }
        
        public static ComplexTestEngineContext CreateAndInitContext()
        {
            return CreateAndInitContext(new UnityTestEngineContextFactorySettings());
        }

        public static ComplexTestEngineContext CreateAndInitContext(UnityTestEngineContextFactorySettings factorySettings)
        {
            var context = CreateContext(factorySettings);
            context.Start();
            return context;
        }
    }
}
