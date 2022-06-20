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

namespace TestSandbox.Handlers
{
    public class SoundStartHandler : BaseGeneralStartHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var platformListener = new TstPlatformHostListener();

            CreateNPC(platformListener);

            var platformListener2 = new TstPlatformHostListener();

            var settings = new GameObjectSettings();

            settings.Id = "#a";
            settings.InstanceId = 2;

            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new Vector3(10, 10, 10);

            //settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Things\Barrel\Barrel.sobj");
            settings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Things\M4A1\M4A1.sobj");
            settings.HostListener = platformListener2;
            settings.PlatformSupport = new PlatformSupportCLIStub(new Vector3(100, 100, 100));

            var gameObject = WorldFactory.WorldInstance.GetGameObject(settings);

            _world.Start();

            Thread.Sleep(5000);

            //gameObject.PushSoundFact(60, "act(M16, shoot)");

            Thread.Sleep(5000);

            _world.Dispose();

            _logger.Log("!---");

            Thread.Sleep(5000);

            _logger.Log("End");
        }
    }
}
