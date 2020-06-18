using NLog;
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

            _logger.Info("End");
        }
    }
}
