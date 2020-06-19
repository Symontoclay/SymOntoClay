using NLog;
using SymOntoClay.CoreHelper.Threads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestSandbox.Threads
{
    public class AsyncActivePeriodicObjectHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var commonActiveContext = new ActivePeriodicObjectCommonContext();

            var activeContext = new ActivePeriodicObjectContext(commonActiveContext);

            var activeObject = new AsyncActivePeriodicObject(activeContext);
            activeObject.PeriodicMethod = NRun;
            activeObject.Start();

            Thread.Sleep(10000);

            _logger.Info($"activeObject.IsWaited = {activeObject.IsWaited}");

            commonActiveContext.Lock();

            Thread.Sleep(1000);

            _logger.Info($"activeObject.IsWaited (2) = {activeObject.IsWaited}");

            Thread.Sleep(1000);

            commonActiveContext.UnLock();

            Thread.Sleep(1000);

            _logger.Info($"activeObject.IsWaited (3) = {activeObject.IsWaited}");

            Thread.Sleep(1000);

            activeObject.Stop();

            Thread.Sleep(1000);

            _logger.Info("End");
        }

        private int _n = 0;

        private bool NRun()
        {
            _n++;

            _logger.Info($"_n = {_n}");

            //if (_n > 10)
            //{
            //    return false;
            //}

            return true;
        }
    }
}
