/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog;
using SymOntoClay.Core.Internal.Threads;
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
