/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

namespace TestSandbox.Handlers
{
    public class GeneralStartHandler: BaseGeneralStartHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var platformListener = new TstPlatformHostListener();

            CreateNPC(platformListener);

            _world.Start();

            //Thread.Sleep(5000);

            //_logger.Log("|||||||||||||");

            //_npc.PushSoundFact(60, "act(M16, shoot)");

            //var factId = _npc.InsertFact("{: see(I, #a) :}");
            //_npc.InsertFact("{: barrel (#a) :}");
            //_npc.InsertFact("distance(I, #a, 14.71526)");
            //_npc.InsertFact("{: see(I, enemy) :}");

            //Thread.Sleep(5000);
            Thread.Sleep(50000);
            //Thread.Sleep(500000);

            _logger.Log("!---");

            _world.Dispose();
            
            _logger.Log("!(+)---");

            //_npc.InsertFact("{: see(I, #b) :}");

            Thread.Sleep(1000);

            //Thread.Sleep(10000);

            //_npc.RemoveFact(factId);

            //Thread.Sleep(50000);

            _logger.Log("End");
        }
    }
}
