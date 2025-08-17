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

using SymOntoClay.BaseTestLib;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Monitor.NLog.PlatformLoggers;
using SymOntoClay.NLP;
using SymOntoClay.ProjectFiles;
using SymOntoClay.SoundBuses;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestSandbox.Helpers
{
    public static class TstEngineContextHelper
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public static WorldSettings CreateWorldSettings(UnityTestEngineContextFactorySettings factorySettings)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;

            
            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);


            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create(factorySettings.CancellationToken);

            var codeDir = Path.Combine(Directory.GetCurrentDirectory(), "Source");


            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = codeDir,
                AppName = $@"SymOntoClay\{appName}"
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            var settings = new WorldSettings();

            settings.CancellationToken = factorySettings.CancellationToken;

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

            settings.SoundBus = new SimpleSoundBus(new SimpleSoundBusSettings
            {
                CancellationToken = factorySettings.CancellationToken,
                ThreadingSettings = factorySettings.ThreadingSettings.AsyncEvents
            });

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
                EnableFullCallInfo = true,
                CancellationToken = factorySettings.CancellationToken,
                ThreadingSettings = factorySettings.ThreadingSettings.AsyncEvents,
                Features = new MonitorFeatures
                {
                    EnableCallMethod = true,
                    EnableParameter = true,
                    EnableEndCallMethod = true,
                    EnableMethodResolving = true,
                    EnableEndMethodResolving = true,
                    EnableActionResolving = true,
                    EnableEndActionResolving = true,
                    EnableHostMethodResolving = true,
                    EnableEndHostMethodResolving = true,
                    EnableHostMethodActivation = true,
                    EnableEndHostMethodActivation = true,
                    EnableHostMethodStarting = true,
                    EnableEndHostMethodStarting = true,
                    EnableHostMethodExecution = true,
                    EnableEndHostMethodExecution = true,
                    EnableSystemExpr = true,
                    EnableCodeFrame = true,
                    EnableLeaveThreadExecutor = true,
                    EnableGoBackToPrevCodeFrame = true,
                    EnableStartProcessInfo = true,
                    EnableCancelProcessInfo = true,
                    EnableWeakCancelProcessInfo = true,
                    EnableCancelInstanceExecution = true,
                    EnableSetExecutionCoordinatorStatus = true,
                    EnableSetProcessInfoStatus = true,
                    EnableWaitProcessInfo = true,
                    EnableRunLifecycleTrigger = true,
                    EnableDoTriggerSearch = true,
                    EnableEndDoTriggerSearch = true,
                    EnableSetConditionalTrigger = true,
                    EnableResetConditionalTrigger = true,
                    EnableRunSetExprOfConditionalTrigger = true,
                    EnableEndRunSetExprOfConditionalTrigger = true,
                    EnableRunResetExprOfConditionalTrigger = true,
                    EnableEndRunResetExprOfConditionalTrigger = true,
                    EnableActivateIdleAction = true,

                    EnableHtn = true,

                    EnableBuildPlan = true,

                    EnableOutput = true,
                    EnableTrace = true,
                    EnableDebug = true,
                    EnableInfo = true,
                    EnableWarn = true,
                    EnableError = true,
                    EnableFatal = true
                }
            };

            if(factorySettings.PlatformLogger != null)
            {
                monitorSettings.PlatformLoggers = new List<IPlatformLogger>() { factorySettings.PlatformLogger };
            }
            else
            {
                if(factorySettings.UseDefaultPlatformLogger)
                {
                    monitorSettings.PlatformLoggers = new List<IPlatformLogger>() { /*ConsoleLogger.Instance,*/ CommonNLogPlatformLogger.Instance };
                }
            }

            settings.Monitor = new SymOntoClay.Monitor.Monitor(monitorSettings);

            settings.ThreadingSettings = factorySettings.ThreadingSettings;

            settings.HtnExecutionSettings = factorySettings.HtnExecutionSettings;

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

            npcSettings.ThreadingSettings = factorySettings.ThreadingSettings;

            npcSettings.HtnExecutionSettings = factorySettings.HtnExecutionSettings;

            return npcSettings;
        }

        public static IWorld CreateWorld(UnityTestEngineContextFactorySettings factorySettings)
        {
            var settings = CreateWorldSettings(factorySettings);

#if DEBUG
            _logger.Info("9A63D388-144D-433E-A3B6-96A7BD3A9EB7", $"settings = {settings}");
#endif

            return UnityTestEngineContextFactory.CreateWorld(settings);
        }

        public static IHumanoidNPC CreateNPC(IWorld world, UnityTestEngineContextFactorySettings factorySettings)
        {
            var npcSettings = CreateHumanoidNPCSettings(factorySettings);

#if DEBUG
            _logger.Info("5881B7D8-D150-4C42-9DA0-9F4781F4BBA5", $"npcSettings = {npcSettings}");
#endif

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
