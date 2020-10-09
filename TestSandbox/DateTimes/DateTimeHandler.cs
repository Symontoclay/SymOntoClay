/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
