/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Threads
{
    public class AsyncActivePeriodicObjectHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

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

            _logger.Log($"activeObject.IsWaited (0) = {activeObject.IsWaited}");
            _logger.Log($"activeObject.IsActive (0) = {activeObject.IsActive}");

            var taskValue = activeObject.Start();

            _logger.Log($"taskValue = {taskValue}");

            activeObject2.Start();

            Thread.Sleep(10000);

            _logger.Log($"activeObject.IsWaited = {activeObject.IsWaited}");
            _logger.Log($"activeObject.IsActive = {activeObject.IsActive}");

            commonActiveContext.Lock();

            activeContext.WaitWhenAllIsNotWaited();

            Thread.Sleep(1000);

            _logger.Log($"activeObject.IsWaited (2) = {activeObject.IsWaited}");
            _logger.Log($"activeObject.IsActive (2) = {activeObject.IsActive}");

            Thread.Sleep(1000);

            commonActiveContext.UnLock();

            Thread.Sleep(1000);

            _logger.Log($"activeObject.IsWaited (3) = {activeObject.IsWaited}");
            _logger.Log($"activeObject.IsActive (3) = {activeObject.IsActive}");

            //Thread.Sleep(1000);

            //activeObject.Stop();

            //_logger.Log($"activeObject.IsWaited (4) = {activeObject.IsWaited}");
            //_logger.Log($"activeObject.IsActive (4) = {activeObject.IsActive}");

            //Thread.Sleep(10000);

            //taskValue = activeObject.Start();

            //_logger.Log($"taskValue = {taskValue}");

            //Thread.Sleep(1000);

            //activeObject.Stop();

            //Thread.Sleep(1000);

            //_logger.Log($"activeObject.TaskValue = {activeObject.TaskValue}");

            //activeObject.Dispose();

            //Thread.Sleep(1000);

            //taskValue = activeObject.Start();

            //_logger.Log($"taskValue = {taskValue}");

            Thread.Sleep(1000);

            _logger.Log("End");
        }

        private int _n = 0;
        private int _m = 0;

        private bool NRun()
        {
            _n++;

            _logger.Log($"_n = {_n}");

            //if (_n > 10)
            //{
            //    return false;
            //}

            return true;
        }

        private bool NRun_2()
        {
            _m++;

            _logger.Log($"_m = {_m}");

            //if (_n > 10)
            //{
            //    return false;
            //}

            return true;
        }
    }
}
