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
using SymOntoClay.BaseTestLib;
using SymOntoClay.SoundBuses;
using SymOntoClay.StandardFacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.Helpers;

namespace TestSandbox.SoundBusHandler
{
    public class TstSoundBusHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case3()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;

            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var bus = new SimpleSoundBus();

            var receiver1 = new TstSoundReceiver(11, new Vector3(10, 10, 10));
            bus.AddReceiver(receiver1);

            var factStr = "{: act(M16, shoot) :}";
            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info($"fact = '{fact.ToHumanizedString()}'");

            bus.PushSound(12, 60, new Vector3(1, 1, 1), fact);

            Thread.Sleep(1000);
        }

        private void Case2()
        {
            var bus = new SimpleSoundBus();
            
            var receiver1 = new TestSoundReceiver(11, new Vector3(10, 10, 10), (double power, double distance, Vector3 position, string query, string convertedQuery) => {
                _logger.Info($"power = {power}");
                _logger.Info($"distance = {distance}");
                _logger.Info($"position = {position}");
                _logger.Info($"query = {query}");
                _logger.Info($"convertedQuery = {convertedQuery}");
            });

            bus.AddReceiver(receiver1);

            bus.PushSound(12, 60, new Vector3(1, 1, 1), "act(M16, shoot)");

            Thread.Sleep(100);
        }

        private void Case1()
        {
            var bus = new SimpleSoundBus();

            var receiver1 = new TstSoundReceiver(11, new Vector3(10, 10, 10));

            bus.AddReceiver(receiver1);

            bus.PushSound(12, 60, new Vector3(1, 1, 1), "act(M16, shoot)");
            //bus.PushSound(12, 60, new Vector3(1, 1, 1), "direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self)");

            Thread.Sleep(1000);
        }
    }
}
