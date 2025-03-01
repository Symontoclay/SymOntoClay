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
using SymOntoClay.UnityAsset.Core.Internal.DateAndTime;
using System.Threading;

namespace TestSandbox.DateTimes
{
    public class DateTimeHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("318FB3BC-0385-4398-B412-9F7912CD4FAA", "Begin");

            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20);

            var commonActiveContext = new ActiveObjectCommonContext();

            var dateTimeProvider = new DateTimeProvider(_logger, commonActiveContext, threadPool, cancellationTokenSource.Token);

            dateTimeProvider.LoadFromSourceCode();
            dateTimeProvider.Start();

            Thread.Sleep(10000);

            _logger.Info("CA8ABEE8-6991-49CA-B002-DAAC33B90B1A", $"dateTimeProvider.CurrentTicks = {dateTimeProvider.CurrentTicks}");

            _logger.Info("7250B82C-E27B-4139-BD88-EF70629095D4", "End");
        }
    }
}
