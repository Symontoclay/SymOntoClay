/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using SymOntoClay.SoundBuses;

namespace TestSandbox.Helpers
{
    public static class TstEngineContextHelper
    {
        //private static readonly IEntityLogger _logger = new LoggerImpementation();

        public static WorldSettings CreateWorldSettings(bool withAppFiles)
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;

            //_logger.Log($"appName = {appName}");

            var supportBasePath = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", appName);

            //_logger.Log($"supportBasePath = {supportBasePath}");

            var logDir = Path.Combine(supportBasePath, "NpcLogs");

            var invokingInMainThread = DefaultInvokerInMainThreadFactory.Create();

            var settings = new WorldSettings();
            settings.EnableAutoloadingConvertors = true;

            settings.SharedModulesDirs = new List<string>() { Path.Combine(Directory.GetCurrentDirectory(), "Source", "Modules") };

            settings.ImagesRootDir = Path.Combine(supportBasePath, "Images");

            settings.BuiltInStandardLibraryDir = DefaultPaths.GetBuiltInStandardLibraryDir();

            settings.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "SymOntoClay", appName);

            if(withAppFiles)
            {
                settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\World\World.world");
            }            

            settings.InvokerInMainThread = invokingInMainThread;

            settings.SoundBus = new SimpleSoundBus();

            settings.Logging = new LoggingSettings()
            {
                LogDir = logDir,
                RootContractName = "Hi1",
                PlatformLoggers = new List<IPlatformLogger>() { /*ConsoleLogger.Instance,*/ CommonNLogLogger.Instance },
                Enable = true,
                EnableRemoteConnection = true
            };

            return settings;
        }

        public static HumanoidNPCSettings CreateHumanoidNPCSettings(bool withAppFiles)
        {
            return CreateHumanoidNPCSettings(UnityTestEngineContextFactory.DefaultPlatformListener, withAppFiles);
        }

        public static HumanoidNPCSettings CreateHumanoidNPCSettings(object hostListener, bool withAppFiles)
        {
            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = "#020ED339-6313-459A-900D-92F809CEBDC5";
            npcSettings.InstanceId = 1;
            
            if(withAppFiles)
            {
                npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PeaceKeeper\PeaceKeeper.sobj");
            }
            
            npcSettings.HostListener = hostListener;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            return npcSettings;
        }

        public static ComplexTestEngineContext CreateContext(bool withAppFiles = true)
        {
            return CreateContext(UnityTestEngineContextFactory.DefaultPlatformListener, withAppFiles);
        }

        public static ComplexTestEngineContext CreateContext(object hostListener, bool withAppFiles = true)
        {
            var settings = CreateWorldSettings(withAppFiles);

            //_logger.Log($"settings = {settings}");

            var npcSettings = CreateHumanoidNPCSettings(hostListener, withAppFiles);

            //_logger.Log($"npcSettings = {npcSettings}");

            return UnityTestEngineContextFactory.CreateTestEngineContext(settings, npcSettings);
        }

        public static ComplexTestEngineContext CreateAndInitContext(bool withAppFiles = false)
        {
            var context = CreateContext(withAppFiles);
            context.Start();
            return context;
        }

        public static ComplexTestEngineContext CreateAndInitContextWithoutAppFiles()
        {
            return CreateAndInitContext(false);
        }
    }
}
