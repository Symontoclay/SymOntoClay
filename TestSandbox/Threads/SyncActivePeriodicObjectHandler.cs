using NLog;
using SymOntoClay.CoreHelper.Threads;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.Threads
{
    public class SyncActivePeriodicObjectHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var activeObject = new SyncActivePeriodicObject();
            activeObject.PeriodicMethod = NRun;
            activeObject.Start();

            _logger.Info("End");
        }

        private int _n = 0;

        private bool NRun()
        {
            _n++;

            _logger.Info($"_n = {_n}");

            if (_n > 10)
            {
                return false;
            }

            return true;
        }
    }
}
