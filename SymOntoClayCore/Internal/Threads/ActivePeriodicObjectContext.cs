/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Threads
{
    public class ActivePeriodicObjectContext : IActivePeriodicObjectContext
    {
        public ActivePeriodicObjectContext(IActivePeriodicObjectCommonContext commonContext)
        {
            _commonContext = commonContext;
        }

        private readonly IActivePeriodicObjectCommonContext _commonContext;

        /// <inheritdoc/>
        public bool IsNeedWating => _commonContext.IsNeedWating;

        /// <inheritdoc/>
        public EventWaitHandle WaitEvent => _commonContext.WaitEvent;

        private object _lockObj = new object();

        private List<IActivePeriodicObject> _children = new List<IActivePeriodicObject>();

        /// <inheritdoc/>
        void IActivePeriodicObjectContext.AddChildActiveObject(IActivePeriodicObject activeObject)
        {
            lock (_lockObj)
            {
                if (_children.Contains(activeObject))
                {
                    return;
                }

                _children.Add(activeObject);
            }
        }

        /// <inheritdoc/>
        void IActivePeriodicObjectContext.RemoveChildActiveObject(IActivePeriodicObject activeObject)
        {
            lock (_lockObj)
            {
                if (_children.Contains(activeObject))
                {
                    _children.Remove(activeObject);
                }
            }
        }

        /// <inheritdoc/>
        public void WaitWhenAllIsNotWaited()
        {
            lock (_lockObj)
            {
                while (!_children.All(p => p.IsWaited))
                {
                }
            }
        }

        /// <inheritdoc/>
        public void StartAll()
        {
            lock (_lockObj)
            {
                foreach (var child in _children)
                {
                    child.Start();
                }
            }
        }

        /// <inheritdoc/>
        public void StopAll()
        {
            lock (_lockObj)
            {
                foreach (var child in _children)
                {
                    child.Stop();
                }
            }
        }
    }
}
