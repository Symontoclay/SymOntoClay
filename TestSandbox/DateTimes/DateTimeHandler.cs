using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.DateAndTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.DateTimes
{
    public class DateTimeHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var commonActiveContext = new ActivePeriodicObjectCommonContext();

            var dateTimeProvider = new DateTimeProvider(_logger, commonActiveContext);

            dateTimeProvider.LoadFromSourceCode();
            dateTimeProvider.Start();

            Thread.Sleep(10000);

            _logger.Log($"dateTimeProvider.CurrentTiks = {dateTimeProvider.CurrentTiks}");

            _logger.Log("End");
        }
    }
}
