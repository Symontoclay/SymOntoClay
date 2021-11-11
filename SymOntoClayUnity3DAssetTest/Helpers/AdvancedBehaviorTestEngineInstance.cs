/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClayProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Numerics;
using NLog;
using SymOntoClay.SoundBuses;

namespace SymOntoClay.UnityAsset.Core.Tests.Helpers
{
    public class AdvancedBehaviorTestEngineInstance : IDisposable
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        static AdvancedBehaviorTestEngineInstance()
        {
            _rootDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), $"TstTempProjects_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_rootDir))
            {
                Directory.CreateDirectory(_rootDir);
            }
        }

        private static string _rootDir;

        public static string RoorDir => _rootDir;

        public AdvancedBehaviorTestEngineInstance()
            : this(RoorDir)
        {
        }

        public AdvancedBehaviorTestEngineInstance(string rootDir)
        {
            _testDir = Path.Combine(rootDir, $"TstDir{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_testDir))
            {
                Directory.CreateDirectory(_testDir);
            }

            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() { CreateOnlyWorldspace = true, ProjectName = _projectName };

            var wSpaceFile = WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, _testDir
                    , errorMsg => throw new Exception(errorMsg)
                    );

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
            if(!_createdThingsDSLProjects.Contains(thingName))
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

        public IWorld CreateWorld(Action<int, string> logChannel, bool enableWriteLnRawLog = false)
        {
            var n = 0;

            return CreateWorld(message => { n++; logChannel(n, message); },
                error => { throw new Exception(error); }, enableWriteLnRawLog);
        }

        public IWorld CreateWorld(Action<string> logChannel, Action<string> error, bool enableWriteLnRawLog = false)
        {
            var supportBasePath = Path.Combine(_testDir, "SysDirs");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var world = new WorldCore();

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.SharedModulesDirs = new List<string>() { Path.Combine(_wSpaceDir, "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(supportBasePath, "TMP");

            settings.HostFile = Path.Combine(_wSpaceDir, "World/World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            var callBackLogger = new CallBackLogger(
                message => { logChannel(message); },
                errorMsg => { error(errorMsg); },
                enableWriteLnRawLog);

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { callBackLogger },
                Enable = true,
                EnableRemoteConnection = true
            };

            world.SetSettings(settings);

            return world;
        }

        private void CreateNPCDSLProject(string npcName)
        {
#if DEBUG
            //_logger.Info($"npcName = {npcName}");
            //_logger.Info($"_wSpaceDir = {_wSpaceDir}");
#endif

            var wSpaceFile = WFilesSearcher.FindWSpaceFile(_wSpaceDir);

#if DEBUG
            //_logger.Info($"wSpaceFile.FullName = {wSpaceFile.FullName}");
#endif

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

        public IHumanoidNPC CreateNPC(IWorld world)
        {
            return CreateNPC(world, _projectName, new object(), new Vector3(10, 10, 10));
        }

        public IHumanoidNPC CreateNPC(IWorld world, object platformListener)
        {
            return CreateNPC(world, _projectName, platformListener, new Vector3(10, 10, 10));
        }

        public IHumanoidNPC CreateNPC(IWorld world, string npcName, object platformListener)
        {
            return CreateNPC(world, npcName, platformListener, new Vector3(10, 10, 10));
        }

        public IHumanoidNPC CreateNPC(IWorld world, string npcName, object platformListener, Vector3 currentAbsolutePosition)
        {
            ILoggedTestHostListener loggedTestHostListener = null;

            if (platformListener != null)
            {
                loggedTestHostListener = platformListener as ILoggedTestHostListener;
            }

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = $"#{Guid.NewGuid():D}";
            npcSettings.InstanceId = GetInstanceId();
            npcSettings.LogicFile = Path.Combine(_wSpaceDir, $"Npcs/{npcName}/{npcName}.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub(currentAbsolutePosition);

            var npc = world.GetHumanoidNPC(npcSettings);

            loggedTestHostListener?.SetLogger(npc.Logger);

            return npc;
        }

        public IHumanoidNPC CreateAndStartNPC(Action<int, string> logChannel)
        {
            return CreateAndStartNPC(logChannel, new object());
        }

        public IHumanoidNPC CreateAndStartNPC(Action<int, string> logChannel, object platformListener)
        {
            var n = 0;

            return CreateAndStartNPC(message => { n++; logChannel(n, message); },
                error => { throw new Exception(error); }, platformListener);
        }

        public IHumanoidNPC CreateAndStartNPC(Action<string> logChannel, Action<string> error)
        {
            return CreateAndStartNPC(logChannel, error, new object());
        }

        public IHumanoidNPC CreateAndStartNPC(Action<string> logChannel, Action<string> error, object platformListener)
        {
            ILoggedTestHostListener loggedTestHostListener = null;

            if (platformListener != null)
            {
                loggedTestHostListener = platformListener as ILoggedTestHostListener;
            }

            var world = CreateWorld(logChannel, error, loggedTestHostListener != null);

            var npc = CreateNPC(world, _projectName, platformListener);

            world.Start();

            return npc;
        }

        public IGameObject CreateThing(IWorld world, string thingName, Vector3 currentAbsolutePosition)
        {
            return CreateThing(world, thingName, new object(), currentAbsolutePosition);
        }

        public IGameObject CreateThing(IWorld world, string thingName, object platformListener, Vector3 currentAbsolutePosition)
        {
            var settings = new GameObjectSettings();

            settings.Id = $"#{Guid.NewGuid():D}";
            settings.InstanceId = GetInstanceId();

            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = currentAbsolutePosition;

            settings.HostFile = Path.Combine(_wSpaceDir, $"Things/{thingName}/{thingName}.sobj");
            settings.HostListener = platformListener;
            settings.PlatformSupport = new PlatformSupportCLIStub(currentAbsolutePosition);

            var gameObject = world.GetGameObject(settings);

            return gameObject;
        }

        //private string _defaultRelativeFileName = @"/Npcs/Example/Example.soc";
        private readonly string _projectName = "Example";
        private readonly string _testDir;
        private readonly string _wSpaceDir;

        private readonly List<string> _createdNPCsDSLProjects = new List<string>();
        private readonly List<string> _createdThingsDSLProjects = new List<string>();

        private object _lockObj = new object();

        private int _currInstanceId;

        private int GetInstanceId()
        {
            lock(_lockObj)
            {
                _currInstanceId++;
                return _currInstanceId;
            }
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

            Directory.Delete(_testDir, true);
        }
    }
}
