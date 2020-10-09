/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Threads
{
    public class SyncActivePeriodicObject : IActivePeriodicObject
    {
#if DEBUG
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public PeriodicDelegate PeriodicMethod { get; set; }

        private volatile bool _isActive;

        /// <inheritdoc/>
        public bool IsActive => _isActive;

        /// <inheritdoc/>
        public bool IsWaited => false;

        private Value _taskValue = new NullValue();

        /// <inheritdoc/>
        public Value TaskValue => _taskValue;

        /// <inheritdoc/>
        public Value Start()
        {
            _isActive = true;

            while (true)
            {
                if (!PeriodicMethod())
                {
                    _isActive = false;
                    return _taskValue;
                }
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
