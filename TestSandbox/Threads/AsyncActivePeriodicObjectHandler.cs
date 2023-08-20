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
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Threads
{
    public class AsyncActivePeriodicObjectHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("Begin");

            var commonActiveContext = new ActivePeriodicObjectCommonContext();

            var activeContext = new ActivePeriodicObjectContext(commonActiveContext);

            var activeObject = new AsyncActivePeriodicObject(activeContext)
            {
                PeriodicMethod = NRun
            };

            var activeObject2 = new AsyncActivePeriodicObject(activeContext)
            {
                PeriodicMethod = NRun_2
            };

            _logger.Info($"activeObject.IsWaited (0) = {activeObject.IsWaited}");
            _logger.Info($"activeObject.IsActive (0) = {activeObject.IsActive}");

            var taskValue = activeObject.Start();

            _logger.Info($"taskValue = {taskValue}");

            activeObject2.Start();

            Thread.Sleep(10000);

            _logger.Info($"activeObject.IsWaited = {activeObject.IsWaited}");
            _logger.Info($"activeObject.IsActive = {activeObject.IsActive}");

            commonActiveContext.Lock();

            activeContext.WaitWhenAllIsNotWaited();

            Thread.Sleep(1000);

            _logger.Info($"activeObject.IsWaited (2) = {activeObject.IsWaited}");
            _logger.Info($"activeObject.IsActive (2) = {activeObject.IsActive}");

            Thread.Sleep(1000);

            commonActiveContext.UnLock();

            Thread.Sleep(1000);

            _logger.Info($"activeObject.IsWaited (3) = {activeObject.IsWaited}");
            _logger.Info($"activeObject.IsActive (3) = {activeObject.IsActive}");















            Thread.Sleep(1000);

            _logger.Info("End");
        }

        private int _n = 0;
        private int _m = 0;

        private bool NRun(CancellationToken cancellationToken)
        {
            _n++;

            _logger.Info($"_n = {_n}");


            return true;
        }

        private bool NRun_2(CancellationToken cancellationToken)
        {
            _m++;

            _logger.Info($"_m = {_m}");


            return true;
        }
    }
}
