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
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TestSandbox.CoreHostListener;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;
using SymOntoClay.BaseTestLib.HostListeners;
using System.Runtime;
using SymOntoClay.BaseTestLib;

namespace TestSandbox.Handlers
{
    public class GeneralStartHandler: BaseGeneralStartHandler
    {
        public GeneralStartHandler()
            : base(new UnityTestEngineContextFactorySettings()
            {
                UseDefaultNLPSettings = true
            })
        {
        }

        public void Run()
        {
            _logger.Log("Begin");

            var platformListener = new FullGeneralized_Tests_HostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            factorySettings.Categories = new List<string>() { "elf" };
            factorySettings.EnableCategories = true;

            CreateMainNPC(factorySettings);

            _world.Start();

            Thread.Sleep(100);

            _logger.Log("|||||||||||||");

            _npc.Logger.LogChannel("|||||||||||||");

            Thread.Sleep(500);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            _npc.EngineContext.Storage.InsertListenedFact(factStr);

            var factId = _npc.InsertFact("{: see(I, #a) :}");

            _npc.Logger.LogChannel("|-|-|-|-|-|-|-|-|-|-|-|-|");
            _logger.Log("|-|-|-|-|-|-|-|-|-|-|-|-|");

            Thread.Sleep(50000);

            _logger.Log("End");
        }
    }
}
