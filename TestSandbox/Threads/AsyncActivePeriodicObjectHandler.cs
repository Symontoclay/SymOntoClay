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

using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System.Threading;

namespace TestSandbox.Threads
{
    public class AsyncActivePeriodicObjectHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("9E0B3523-0D26-4864-A12B-D953C36A6BA2", "Begin");

            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var activeObject = new AsyncActivePeriodicObject(activeContext, threadPool, _logger)
            {
                PeriodicMethod = NRun
            };

            var activeObject2 = new AsyncActivePeriodicObject(activeContext, threadPool, _logger)
            {
                PeriodicMethod = NRun_2
            };

            _logger.Info("5442E7B9-A695-40B2-BC02-4CD3F5333FB1", $"activeObject.IsWaited (0) = {activeObject.IsWaited}");
            _logger.Info("8B9469EE-FFA5-4E99-898E-D73766BE7360", $"activeObject.IsActive (0) = {activeObject.IsActive}");

            var taskValue = activeObject.Start();

            _logger.Info("4D80C65C-3477-4AA9-B710-2D3991CFD649", $"taskValue = {taskValue}");

            activeObject2.Start();

            Thread.Sleep(10000);

            _logger.Info("EE8ABBDD-6A49-4BD8-8922-FD52CD97F164", $"activeObject.IsWaited = {activeObject.IsWaited}");
            _logger.Info("6C97EA93-73D3-4B67-B6D2-3BB7CBC5353C", $"activeObject.IsActive = {activeObject.IsActive}");

            commonActiveContext.Lock();

            activeContext.WaitWhenAllIsNotWaited();

            Thread.Sleep(1000);

            _logger.Info("ADCF53E3-CB13-4B07-9983-D5DE522C511D", $"activeObject.IsWaited (2) = {activeObject.IsWaited}");
            _logger.Info("8E94A7E7-BF8B-42E9-BFA7-366AFB95C372", $"activeObject.IsActive (2) = {activeObject.IsActive}");

            Thread.Sleep(1000);

            commonActiveContext.UnLock();

            Thread.Sleep(1000);

            _logger.Info("CDAB2113-F5A2-4CD7-8321-579EAA6C57FC", $"activeObject.IsWaited (3) = {activeObject.IsWaited}");
            _logger.Info("9E05155F-3AC4-48A2-9C40-A4842C34A415", $"activeObject.IsActive (3) = {activeObject.IsActive}");















            Thread.Sleep(1000);

            _logger.Info("EA561877-4C5A-43A0-84DE-5E7AE2353275", "End");
        }

        private int _n = 0;
        private int _m = 0;

        private bool NRun(CancellationToken cancellationToken)
        {
            _n++;

            _logger.Info("B32B701D-6F24-43FA-872B-D45F07DE5EC8", $"_n = {_n}");


            return true;
        }

        private bool NRun_2(CancellationToken cancellationToken)
        {
            _m++;

            _logger.Info("4D1A0A5A-3ADA-4001-96C2-472CF4ECE6C1", $"_m = {_m}");


            return true;
        }
    }
}
