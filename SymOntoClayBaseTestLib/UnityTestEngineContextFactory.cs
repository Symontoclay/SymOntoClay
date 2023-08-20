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
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.SoundBuses;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.NLP;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.ProjectFiles;
using SymOntoClay.Monitor.Common;
using SymOntoClay.BaseTestLib.Monitoring;

namespace SymOntoClay.BaseTestLib
{
    public static class UnityTestEngineContextFactory
    {
        public static string CreateRootDir()
        {
            var rootDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), $"TstTempProjects_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }

            return rootDir;
        }

        public static string CreateTestDir(string rootDir)
        {
            var testDir = Path.Combine(rootDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            return testDir;
        }

        public static WorldSettings CreateWorldSettings(UnityTestEngineContextFactorySettings factorySettings)
        {
            var baseDir = factorySettings.BaseDir;

            if (string.IsNullOrWhiteSpace(baseDir) && factorySettings.UseDefaultBaseDir)
            {
                baseDir = CreateTestDir(CreateRootDir());
            }

            var supportBasePath = Path.Combine(baseDir, "SysDirs");

            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(supportBasePath, "TMP");

            if (string.IsNullOrWhiteSpace(factorySettings.WorldFile))
            {
                if (factorySettings.UseStandardLibrary)
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name == "SymOntoClay.BaseTestLib").Location;

                    var assemblyPath = new FileInfo(assembly).DirectoryName;

                    var builtInStandardLibraryDir = Path.Combine(assemblyPath, "LibsForInstall", "stdlib");

                    settings.BuiltInStandardLibraryDir = builtInStandardLibraryDir;
                }
            }
            else
            {
                var worldFile = factorySettings.WorldFile;

                settings.HostFile = worldFile;

                var fileInfo = new FileInfo(worldFile);

                var directory = fileInfo.Directory.Parent;

                var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
                {
                    InputDir = directory.FullName,
                    AppName = "tst",
                    SearchMainNpcFile = false
                };

                var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

                var libsDirs = new List<string>();

                if (!string.IsNullOrWhiteSpace(targetFiles.SharedLibsDir))
                {
                    libsDirs.Add(targetFiles.SharedLibsDir);
                }

                if (!string.IsNullOrWhiteSpace(targetFiles.LibsDir))
                {
                    libsDirs.Add(targetFiles.LibsDir);
                }

                settings.LibsDirs = libsDirs;

                if (factorySettings.UseStandardLibrary)
                {
                    if (string.IsNullOrWhiteSpace(targetFiles.SharedLibsDir))
                    {
                        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name == "SymOntoClay.BaseTestLib").Location;

                        var assemblyPath = new FileInfo(assembly).DirectoryName;

                        var builtInStandardLibraryDir = Path.Combine(assemblyPath, "LibsForInstall", "stdlib");

                        settings.BuiltInStandardLibraryDir = builtInStandardLibraryDir;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            if (!factorySettings.DictsPaths.IsNullOrEmpty() || !factorySettings.DictsList.IsNullOrEmpty() || factorySettings.UseDefaultNLPSettings)
            {
                var nlpConverterProviderSettings = new NLPConverterProviderSettings();

                if (!factorySettings.DictsPaths.IsNullOrEmpty() || !factorySettings.DictsList.IsNullOrEmpty())
                {
                    nlpConverterProviderSettings.DictsPaths = factorySettings.DictsPaths;
                    nlpConverterProviderSettings.DictsList = factorySettings.DictsList;
                }
                else
                {
                    if (factorySettings.UseDefaultNLPSettings)
                    {
                        nlpConverterProviderSettings.DictsList = new List<IWordsDict>() { DictionaryInstance.Instance };
                    }
                }

                nlpConverterProviderSettings.CreationStrategy = CreationStrategy.Singleton;

                var nlpConverterProvider = new NLPConverterProvider(nlpConverterProviderSettings);

                settings.NLPConverterProvider = nlpConverterProvider;
            }

            settings.StandardFactsBuilder = new StandardFactsBuilder();

            var monitorSettings = new SymOntoClay.Monitor.MonitorSettings
            {
                MessagesDir = monitorMessagesDir,
                Enable = true
            };

            if (factorySettings.PlatformLogger == null)
            {
                if (factorySettings.UseDefaultPlatformLogger)
                {
                    monitorSettings.PlatformLoggers = new List<IPlatformLogger>() { new EmptyLogger() };
                }
            }
            else
            {
                monitorSettings.PlatformLoggers = new List<IPlatformLogger>() { factorySettings.PlatformLogger };
            }

            return settings;
        }

        public static IWorld CreateWorld(UnityTestEngineContextFactorySettings factorySettings)
        {
            var settings = CreateWorldSettings(factorySettings);

            return CreateWorld(settings);
        }

        public static IWorld CreateWorld(WorldSettings settings)
        {
            var world = new WorldCore();
            world.SetSettings(settings);

            return world;
        }

        private static object _lockObj = new object();
        private static int _currInstanceId;

        public static int GetInstanceId()
        {
            lock (_lockObj)
            {
                _currInstanceId++;
                return _currInstanceId;
            }
        }

        public static object DefaultPlatformListener => new object();
        public static Vector3 DefaultCurrentAbsolutePosition => new Vector3(10, 10, 10);

        public static HumanoidNPCSettings CreateHumanoidNPCSettings(UnityTestEngineContextFactorySettings factorySettings)/*string logicFile, object platformListener, Vector3 currentAbsolutePosition*/
        {
            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = $"#{Guid.NewGuid():D}";
            npcSettings.InstanceId = GetInstanceId();

            if (!string.IsNullOrWhiteSpace(factorySettings.NPCAppFile))
            {
                npcSettings.LogicFile = factorySettings.NPCAppFile;
            }

            if (factorySettings.HostListener == null)
            {
                if (factorySettings.UseDefaultHostListener)
                {
                    npcSettings.HostListener = DefaultPlatformListener;
                }
            }
            else
            {
                npcSettings.HostListener = factorySettings.HostListener;
            }

            if (factorySettings.CurrentAbsolutePosition.HasValue)
            {
                npcSettings.PlatformSupport = new PlatformSupportCLIStub(factorySettings.CurrentAbsolutePosition.Value);
            }
            else
            {
                if (factorySettings.UseDefaultCurrentAbsolutePosition)
                {
                    npcSettings.PlatformSupport = new PlatformSupportCLIStub(DefaultCurrentAbsolutePosition);
                }
            }

            npcSettings.Categories = factorySettings.Categories;
            npcSettings.EnableCategories = factorySettings.EnableCategories;

            return npcSettings;
        }

        public static IHumanoidNPC CreateHumanoidNPC(IWorld world, UnityTestEngineContextFactorySettings factorySettings)/*IWorld world, string logicFile, object platformListener, Vector3 currentAbsolutePosition*/
        {
            var npcSettings = CreateHumanoidNPCSettings(factorySettings);

            return CreateHumanoidNPC(world, npcSettings);
        }

        public static IHumanoidNPC CreateHumanoidNPC(IWorld world, HumanoidNPCSettings npcSettings)
        {
            var npc = world.GetHumanoidNPC(npcSettings);

            ILoggedTestHostListener loggedTestHostListener = null;

            var platformListener = npcSettings.HostListener;

            if (platformListener != null)
            {
                loggedTestHostListener = platformListener as ILoggedTestHostListener;
            }

            loggedTestHostListener?.SetLogger(npc.Logger);

            return npc;
        }

        public static ComplexTestEngineContext CreateTestEngineContext(UnityTestEngineContextFactorySettings factorySettings)
        {
            var worldSettings = CreateWorldSettings(factorySettings); /*baseDir, string.Empty, entityLogger*/

            var npcSettings = CreateHumanoidNPCSettings(factorySettings);/*string.Empty, DefaultPlatformListener, DefaultCurrentAbsolutePosition*/

            return CreateTestEngineContext(worldSettings, npcSettings);
        }

        public static ComplexTestEngineContext CreateTestEngineContext(WorldSettings worldSettings, HumanoidNPCSettings npcSettings)
        {
            return CreateTestEngineContext(worldSettings, npcSettings, string.Empty);
        }

        public static ComplexTestEngineContext CreateTestEngineContext(WorldSettings worldSettings, HumanoidNPCSettings npcSettings, string baseDir)
        {
            var world = CreateWorld(worldSettings);

            var npc = CreateHumanoidNPC(world, npcSettings);

            return new ComplexTestEngineContext(world, npc, baseDir);
        }

        public static ComplexTestEngineContext CreateAndInitTestEngineContext(UnityTestEngineContextFactorySettings factorySettings)
        {
            var context = CreateTestEngineContext(factorySettings);
            context.Start();
            return context;
        }
    }
}
