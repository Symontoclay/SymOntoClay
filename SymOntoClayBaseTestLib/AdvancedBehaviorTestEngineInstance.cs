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

using SymOntoClay.UnityAsset.Core.World;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.ProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Numerics;
using NLog;
using SymOntoClay.SoundBuses;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.Core;
using SymOntoClay.Threading;

namespace SymOntoClay.BaseTestLib
{
    public class AdvancedBehaviorTestEngineInstance : IDisposable
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        static AdvancedBehaviorTestEngineInstance()
        {
            _rootDir = UnityTestEngineContextFactory.CreateRootDir();
        }

        private static string _rootDir;

        public static string RoorDir => _rootDir;

        public AdvancedBehaviorTestEngineInstance()
            : this(RoorDir, false)
        {
        }

        public AdvancedBehaviorTestEngineInstance(string rootDir)
            : this(rootDir, false)
        {
        }

        public AdvancedBehaviorTestEngineInstance(string rootDir, KindOfUsingStandardLibrary useStandardLibrary)
            : this(rootDir, false, useStandardLibrary, null)
        {
        }

        public AdvancedBehaviorTestEngineInstance(bool enableNLP)
            : this(RoorDir, enableNLP)
        {
        }

        public AdvancedBehaviorTestEngineInstance(string rootDir, bool enableNLP)
            : this(rootDir, enableNLP, KindOfUsingStandardLibrary.None, null)
        {
        }

        public AdvancedBehaviorTestEngineInstance(AdvancedBehaviorTestEngineInstanceSettings advancedBehaviorTestEngineInstanceSettings)
            : this(RoorDir, false, KindOfUsingStandardLibrary.None, advancedBehaviorTestEngineInstanceSettings)
        {
        }

        public AdvancedBehaviorTestEngineInstance(string rootDir, bool enableNLP, KindOfUsingStandardLibrary useStandardLibrary, AdvancedBehaviorTestEngineInstanceSettings advancedBehaviorTestEngineInstanceSettings)
        {
            _advancedBehaviorTestEngineInstanceSettings = advancedBehaviorTestEngineInstanceSettings;
            _enableNLP = enableNLP;
            _useStandardLibrary = useStandardLibrary;
            _testDir = UnityTestEngineContextFactory.CreateTestDir(rootDir);
            
            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { CreateOnlyWorldspace = true, ProjectName = _projectName };

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings,
                _testDir,
                errorMsg => throw new Exception(errorMsg));

            _wSpaceDir = wSpaceFile.DirectoryName;
        }

        public void WriteFile(string fileContent)
        {
            WriteNPCFile(_projectName, fileContent);
        }

        public void WriteNPCFile(string npcName, string fileContent)
        {
            if (!_createdNPCsDSLProjects.Contains(npcName))
            {
                _createdNPCsDSLProjects.Add(npcName);
                CreateNPCDSLProject(npcName);
            }

            WriteFile($"/Npcs/{npcName}/{npcName}.soc", fileContent);
        }

        public void WriteThingFile(string thingName, string fileContent)
        {
            if (!_createdThingsDSLProjects.Contains(thingName))
            {
                _createdThingsDSLProjects.Add(thingName);
                CreateThingDSLProject(thingName);
            }

            WriteFile($"/Things/{thingName}/{thingName}.soc", fileContent);
        }

        public void WriteFile(string relativeFileName, string fileContent)
        {
            if (relativeFileName.StartsWith("/") || relativeFileName.StartsWith("\\"))
            {
                relativeFileName = relativeFileName.Substring(1);
            }

            var targetFileName = Path.Combine(_wSpaceDir, relativeFileName);

            File.WriteAllText(targetFileName, fileContent);
        }

        public void CreateWorld(Action<int, string> logChannel, bool enableWriteLnRawLog = false)
        {
            var n = 0;

            CreateWorld(message => { n++; logChannel(n, message); },
                error => { throw new Exception(error); }, enableWriteLnRawLog);
        }

        public void CreateWorld(Action<string> logChannel, Action<string> error, bool enableWriteLnRawLog = false)
        {
            var hostFile = Path.Combine(_wSpaceDir, "World/World.world");

            var callBackLogger = new CallBackLogger(
                message => { logChannel(message); },
                errorMsg => { error(errorMsg); },
                enableWriteLnRawLog);

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = _enableNLP;
            factorySettings.UseStandardLibrary = _useStandardLibrary;
            factorySettings.BaseDir = _testDir;
            factorySettings.WorldFile = hostFile;
            factorySettings.PlatformLogger = callBackLogger;
            factorySettings.ThreadingSettings = ConfigureThreadingSettings();

            _world = UnityTestEngineContextFactory.CreateWorld(factorySettings);
        }

        private ThreadingSettings ConfigureThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                }
            };
        }

        public void StartWorld()
        {
            _world?.Start();
        }

        private void CreateNPCDSLProject(string npcName)
        {
            var wSpaceFile = WFilesSearcher.FindWSpaceFile(_wSpaceDir);

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = npcName, KindOfNewCommand = KindOfNewCommand.NPC };

            WorldSpaceCreator.CreateWithWSpaceFile(worldSpaceCreationSettings, wSpaceFile
                , errorMsg => throw new Exception(errorMsg));
        }

        private void CreateThingDSLProject(string thingName)
        {
            var wSpaceFile = WFilesSearcher.FindWSpaceFile(_wSpaceDir);

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { ProjectName = thingName, KindOfNewCommand = KindOfNewCommand.Thing };

            WorldSpaceCreator.CreateWithWSpaceFile(worldSpaceCreationSettings, wSpaceFile
                , errorMsg => throw new Exception(errorMsg));
        }

        public IHumanoidNPC CreateNPC()
        {
            return CreateNPC(_projectName, UnityTestEngineContextFactory.DefaultPlatformListener, UnityTestEngineContextFactory.DefaultCurrentAbsolutePosition, null);
        }

        public IHumanoidNPC CreateNPC(object platformListener)
        {
            return CreateNPC(_projectName, platformListener, UnityTestEngineContextFactory.DefaultCurrentAbsolutePosition, null);
        }

        public IHumanoidNPC CreateNPC(string npcName, object platformListener, int? htnPlanExecutionIterationsMaxCount)
        {
            return CreateNPC(npcName, platformListener, UnityTestEngineContextFactory.DefaultCurrentAbsolutePosition, htnPlanExecutionIterationsMaxCount);
        }

        public IHumanoidNPC CreateNPC(string npcName, object platformListener, Vector3 currentAbsolutePosition, int? htnPlanExecutionIterationsMaxCount)
        {
            return CreateNPC(npcName, platformListener, currentAbsolutePosition, null, htnPlanExecutionIterationsMaxCount);
        }

        public IHumanoidNPC CreateNPC(string npcName, object platformListener, Vector3 currentAbsolutePosition, AdvancedBehaviorTestEngineInstanceSettings advancedBehaviorTestEngineInstanceSettings, int? htnPlanExecutionIterationsMaxCount)
        {
#if DEBUG
            //_logger.Info($"platformListener?.GetType()?.FullName = {platformListener?.GetType()?.FullName}");
#endif

            var logicFile = Path.Combine(_wSpaceDir, $"Npcs/{npcName}/{npcName}.sobj");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.NPCAppFile = logicFile;
            factorySettings.HostListener = platformListener;
            factorySettings.CurrentAbsolutePosition = currentAbsolutePosition;

            if (advancedBehaviorTestEngineInstanceSettings == null)
            {
                if (_advancedBehaviorTestEngineInstanceSettings != null)
                {
                    factorySettings.Categories = _advancedBehaviorTestEngineInstanceSettings.Categories;
                    factorySettings.EnableCategories = _advancedBehaviorTestEngineInstanceSettings.EnableCategories;
                }
            }
            else
            {
                factorySettings.Categories = advancedBehaviorTestEngineInstanceSettings.Categories;
                factorySettings.EnableCategories = advancedBehaviorTestEngineInstanceSettings.EnableCategories;
            }

            factorySettings.ThreadingSettings = ConfigureThreadingSettings();

#if DEBUG
            //_logger.Info($"htnPlanExecutionIterationsMaxCount = {htnPlanExecutionIterationsMaxCount}");
#endif

            if (htnPlanExecutionIterationsMaxCount.HasValue)
            {
                factorySettings.HtnExecutionSettings = new HtnExecutionSettings
                {
                    PlanExecutionIterationsMaxCount = htnPlanExecutionIterationsMaxCount
                };
            }

            return UnityTestEngineContextFactory.CreateHumanoidNPC(_world, factorySettings);
        }

        public IHumanoidNPC CreateAndStartNPC(Action<int, string> logChannel)
        {
            return CreateAndStartNPC(logChannel, new object());
        }

        public IHumanoidNPC CreateAndStartNPC(Action<int, string> logChannel, object platformListener)
        {
            var n = 0;

            return CreateAndStartNPC(message => { n++; logChannel(n, message); },
                error => 
                { 
                    throw new Exception(error); 
                }, platformListener, 
                null);
        }

        public IHumanoidNPC CreateAndStartNPC(Action<string> logChannel, Action<string> error)
        {
            return CreateAndStartNPC(logChannel, error, new object(), null);
        }

        public IHumanoidNPC CreateAndStartNPC(Action<string> logChannel, Action<string> error, object platformListener, int? htnPlanExecutionIterationsMaxCount)
        {
            ILoggedTestHostListener loggedTestHostListener = null;

            if (platformListener != null)
            {
                loggedTestHostListener = platformListener as ILoggedTestHostListener;
            }

            CreateWorld(logChannel, error, loggedTestHostListener != null);

#if DEBUG
            //_logger.Info($"platformListener == null = {platformListener == null}");
#endif

            var npc = CreateNPC(_projectName, platformListener, htnPlanExecutionIterationsMaxCount);

#if DEBUG
            //_logger.Info($"npc.Id = {npc.Id}");
#endif

            StartWorld();

            return npc;
        }

        public IHumanoidNPC GetHumanoidNPC(HumanoidNPCSettings settings)
        {
            return _world.GetHumanoidNPC(settings);
        }

        public IGameObject CreateThing(string thingName)
        {
            return CreateThing(thingName, new object(), new Vector3(10, 10, 10));
        }

        public IGameObject CreateThing(string thingName, Vector3 currentAbsolutePosition)
        {
            return CreateThing(thingName, new object(), currentAbsolutePosition);
        }

        public IGameObject CreateThing(string thingName, object platformListener, Vector3 currentAbsolutePosition)
        {
            return CreateThing(thingName, platformListener, currentAbsolutePosition, null);
        }

        public IGameObject CreateThing(string thingName, object platformListener, Vector3 currentAbsolutePosition, AdvancedBehaviorTestEngineInstanceSettings? advancedBehaviorTestEngineInstanceSettings)
        {
            var settings = new GameObjectSettings();

            settings.Id = $"#{Guid.NewGuid():D}";
            settings.InstanceId = UnityTestEngineContextFactory.GetInstanceId();

            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = currentAbsolutePosition;

            settings.HostFile = Path.Combine(_wSpaceDir, $"Things/{thingName}/{thingName}.sobj");
            settings.HostListener = platformListener;
            settings.PlatformSupport = new PlatformSupportCLIStub(currentAbsolutePosition);

            if (advancedBehaviorTestEngineInstanceSettings == null)
            {
                if (_advancedBehaviorTestEngineInstanceSettings != null)
                {
                    settings.Categories = _advancedBehaviorTestEngineInstanceSettings.Categories;
                    settings.EnableCategories = _advancedBehaviorTestEngineInstanceSettings.EnableCategories;
                }
            }
            else
            {
                settings.Categories = advancedBehaviorTestEngineInstanceSettings.Categories;
                settings.EnableCategories = advancedBehaviorTestEngineInstanceSettings.EnableCategories;
            }

            var gameObject = _world.GetGameObject(settings);

            return gameObject;
        }

        public IPlace GetPlace(PlaceSettings settings)
        {
            return _world.GetPlace(settings);
        }

        private readonly string _projectName = "Example";
        private readonly string _testDir;
        private readonly bool _enableNLP;
        private readonly KindOfUsingStandardLibrary _useStandardLibrary = KindOfUsingStandardLibrary.None;
        private readonly string _wSpaceDir;
        private IWorld _world;
        private readonly AdvancedBehaviorTestEngineInstanceSettings _advancedBehaviorTestEngineInstanceSettings;

        private readonly List<string> _createdNPCsDSLProjects = new List<string>();
        private readonly List<string> _createdThingsDSLProjects = new List<string>();

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            _world?.Dispose();

            Directory.Delete(_testDir, true);
        }
    }
}
