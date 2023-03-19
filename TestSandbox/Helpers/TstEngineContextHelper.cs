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
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.Internal.Logging;
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
using SymOntoClay.Core.Tests.Helpers;
using SymOntoClay.NLP;
using SymOntoClay.StandardFacts;
using SymOntoClayBaseTestLib.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.ProjectFiles;

namespace TestSandbox.Helpers
{
    public static class TstEngineContextHelper
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public static WorldSettings CreateWorldSettings(UnityTestEngineContextFactorySettings factorySettings)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;

            //_logger.Log($"appName = {appName}");

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            //_logger.Log($"supportBasePath = {supportBasePath}");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var codeDir = Path.Combine(Directory.GetCurrentDirectory(), "Source");

            //_logger.Log($"codeDir = {codeDir}");

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = codeDir,
                AppName = $@"SymOntoClay\{appName}"
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

#if DEBUG
            //_logger.Log($"targetFiles = {targetFiles}");
#endif 

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

#if DEBUG
                        //_logger.Log($"mainDictPath = {mainDictPath}");
#endif

                        nlpConverterProviderSettings.DictsPaths = new List<string>() { mainDictPath };
                    }
                }
                
                nlpConverterProviderSettings.CreationStrategy = CreationStrategy.Singleton;

#if DEBUG
                //_logger.Log($"nlpConverterProviderSettings = {nlpConverterProviderSettings}");
#endif

                var nlpConverterProvider = new NLPConverterProvider(nlpConverterProviderSettings);

                settings.NLPConverterProvider = nlpConverterProvider;
            }

            settings.StandardFactsBuilder = new StandardFactsBuilder();

            var loggingSettings = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                Enable = true,
                EnableRemoteConnection = true,
                //KindOfLogicalSearchExplain = KindOfLogicalSearchExplain.DumpAlways,
                KindOfLogicalSearchExplain = KindOfLogicalSearchExplain.None,
                LogicalSearchExplainDumpDir = Directory.GetCurrentDirectory(),
                EnableAddingRemovingFactLoggingInStorages = false
            };

            if(factorySettings.PlatformLogger != null)
            {
                loggingSettings.PlatformLoggers = new List<IPlatformLogger>() { factorySettings.PlatformLogger };
            }
            else
            {
                if(factorySettings.UseDefaultPlatformLogger)
                {
                    loggingSettings.PlatformLoggers = new List<IPlatformLogger>() { /*ConsoleLogger.Instance,*/ CommonNLogLogger.Instance };
                }
            }

            settings.Logging = loggingSettings;

#if DEBUG
            //_logger.Log($"settings = {settings}");
#endif

            return settings;
        }

        public static HumanoidNPCSettings CreateHumanoidNPCSettings(UnityTestEngineContextFactorySettings factorySettings)/*object hostListener, bool withAppFiles*/
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

            //tmp
            npcSettings.Categories = new List<string>() { "elf" };
            //tmp

            return npcSettings;
        }

        public static ComplexTestEngineContext CreateContext(UnityTestEngineContextFactorySettings factorySettings)/*object hostListener, bool withAppFiles = true*/
        {
            var settings = CreateWorldSettings(factorySettings);

            //_logger.Log($"settings = {settings}");

            var npcSettings = CreateHumanoidNPCSettings(factorySettings);

            //_logger.Log($"npcSettings = {npcSettings}");

            return UnityTestEngineContextFactory.CreateTestEngineContext(settings, npcSettings);
        }

        public static ComplexTestEngineContext CreateAndInitContext()
        {
            return CreateAndInitContext(new UnityTestEngineContextFactorySettings());
        }

        public static ComplexTestEngineContext CreateAndInitContext(UnityTestEngineContextFactorySettings factorySettings)/*bool withAppFiles = true*/
        {
            var context = CreateContext(factorySettings);
            context.Start();
            return context;
        }
    }
}
