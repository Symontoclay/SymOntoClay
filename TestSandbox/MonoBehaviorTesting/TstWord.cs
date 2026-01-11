/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Monitor.NLog.PlatformLoggers;
using SymOntoClay.SoundBuses;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TestSandbox.MonoBehaviorTesting
{
    public class TstWord: TstMonoBehaviour
    {
        private readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        private IWorld _world;

        private CancellationTokenSource _cancellationTokenSource;

        public override void Awake()
        {
            _logger.Info("A5E4C33A-962D-4EE5-92C4-A6D2D1BCA31B", "Begin");

            _cancellationTokenSource = new CancellationTokenSource();

            var appName = AppDomain.CurrentDomain.FriendlyName;

            _logger.Info("C5000A3A-FC77-4395-A6BB-21E1582AC6FD", $"appName = {appName}");

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            _logger.Info("235B068B-F202-403D-9D0A-A9A564AB5F8A", $"supportBasePath = {supportBasePath}");

            var monitorMessagesDir = Path.Combine(supportBasePath, "NpcMonitorMessages");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create(_cancellationTokenSource.Token);

            _world = WorldFactory.WorldInstance;

            var settings = new WorldSettings();

            settings.CancellationToken = _cancellationTokenSource.Token;

            settings.LibsDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "SymOntoClay", appName);

            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\World.world");

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus(new SimpleSoundBusSettings
            {
                CancellationToken = _cancellationTokenSource.Token,
                ThreadingSettings = ThreadingSettingsHepler.ConfigureSoundBusThreadingSettings()
            });

            settings.Monitor = new SymOntoClay.Monitor.Monitor(new SymOntoClay.Monitor.MonitorSettings
            {
                MessagesDir = monitorMessagesDir,
                PlatformLoggers = new List<IPlatformLogger>() { ConsolePlatformLogger.Instance, CommonNLogPlatformLogger.Instance },
                Enable = true,
                ThreadingSettings = ThreadingSettingsHepler.ConfigureMonitorThreadingSettings()
            });

            ThreadingSettingsHepler.ConfigureThreadingSettings(settings);

            _logger.Info("B41FD963-D229-4BDF-A3A9-CBF339B120A5", $"settings = {settings}");

            _world.SetSettings(settings);

            _logger.Info("175D394F-43AD-4B26-BCED-E97F79D3D846", "End");
        }

        public override void Start()
        {
            _logger.Info("D582B7B0-A82D-46E2-819C-1655D3873982", "Begin");

            _world.Start();

            _logger.Info("D020A229-22F9-4E6A-92F9-2CC4A985CE65", "End");
        }

        public override void Stop()
        {
            _logger.Info("89DF1C65-5299-4FB4-9AD6-3D323C456C7A", "Begin");

            _cancellationTokenSource.Cancel();
            _world.Dispose();
            _cancellationTokenSource.Dispose();

            _logger.Info("10DCCCD6-E965-4D20-93A8-F168F29AE5DB", "End");
        }
    }
}
