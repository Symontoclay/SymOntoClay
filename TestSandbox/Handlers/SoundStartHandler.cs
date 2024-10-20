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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;
using SymOntoClay.BaseTestLib;

namespace TestSandbox.Handlers
{
    public class SoundStartHandler : BaseGeneralStartHandler
    {
        public SoundStartHandler()
            : base(new UnityTestEngineContextFactorySettings()
            {
                UseDefaultNLPSettings = false
            })
        { 
        }

        public void Run()
        {
            _logger.Info("D1921408-F5AA-4F9A-BA55-BEFCCBF0710A", "Begin");

            var platformListener = new TstPlatformHostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            var platformListener2 = new TstPlatformHostListener();

            var settings = new GameObjectSettings();

            settings.Id = "#a";
            settings.InstanceId = 2;

            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new Vector3(10, 10, 10);

            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Things\M4A1\M4A1.sobj");
            settings.HostListener = platformListener2;
            settings.PlatformSupport = new PlatformSupportCLIStub(new Vector3(100, 100, 100));

            var gameObject = WorldFactory.WorldInstance.GetGameObject(settings);

            _world.Start();

            Thread.Sleep(5000);


            Thread.Sleep(5000);

            _logger.Info("9596A9C7-F3BF-4CF2-BBB7-2FB3085FEC90", "End");
        }
    }
}
