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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;
using SymOntoClay.SoundBuses;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstWord: TstMonoBehaviour
    {
        private readonly IEntityLogger _logger = new LoggerNLogImpementation();

        private IWorld _world;

        public override void Awake()
        {
            _logger.Log("Begin");

            var appName = AppDomain.CurrentDomain.FriendlyName;

            _logger.Log($"appName = {appName}");

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            _logger.Log($"supportBasePath = {supportBasePath}");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            _world = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.LibsDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "SymOntoClay", appName);

            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                PlatformLoggers = new List<IPlatformLogger>() { ConsoleLogger.Instance, CommonNLogLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true
            };

            _logger.Log($"settings = {settings}");

            _world.SetSettings(settings);

            _logger.Log("End");
        }

        public override void Start()
        {
            _logger.Log("Begin");

            _world.Start();

            _logger.Log("End");
        }

        public override void Stop()
        {
            _logger.Log("Begin");

            _world.Dispose();

            _logger.Log("End");
        }
    }
}
