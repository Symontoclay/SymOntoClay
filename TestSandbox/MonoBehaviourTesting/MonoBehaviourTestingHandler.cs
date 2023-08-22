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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class MonoBehaviourTestingHandler
    {
        private readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("38BEAB64-097F-4511-82B5-37FF1530CB1C", "Begin");

            var componentsList = new List<TstMonoBehaviour>() { new TstHumanoidNPC(), new TstWord(), new TstGameObject() };
            ExecuteList(componentsList);

            _logger.Info("B0A32DCB-7F52-44BC-AA3C-B58819DAAF96", "End");
        }

        private void ExecuteList(List<TstMonoBehaviour> componentsList)
        {
            _logger.Info("70700B3E-F972-4C7E-AADA-A3576AB56749", "Begin");

            foreach(var component in componentsList)
            {
                component.Awake();
            }

            foreach (var component in componentsList)
            {
                component.Start();
            }

            Thread.Sleep(5000);

            for (var i = 1; i <= 100; i++)
            {
                foreach (var component in componentsList)
                {
                    component.Update();
                }

                Thread.Sleep(100);
            }

            Thread.Sleep(50000);

            foreach (var component in componentsList)
            {
                component.Stop();
            }

            _logger.Info("03A0E19B-4578-4E81-B4C8-31D15D81CAD4", "End");
        }
    }
}
