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

            var activeObject = new AsyncActivePeriodicObject(activeContext)
            {
                PeriodicMethod = NRun
            };

            _logger.Info($"activeObject.IsWaited (0) = {activeObject.IsWaited}");
            _logger.Info($"activeObject.IsActive (0) = {activeObject.IsActive}");

            activeObject.Start();

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

            activeObject.Stop();

            _logger.Info($"activeObject.IsWaited (4) = {activeObject.IsWaited}");
            _logger.Info($"activeObject.IsActive (4) = {activeObject.IsActive}");

            Thread.Sleep(10000);

            activeObject.Start();

            Thread.Sleep(1000);

            activeObject.Stop();

            Thread.Sleep(1000);

            activeObject.Dispose();

            Thread.Sleep(1000);

            activeObject.Start();

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
