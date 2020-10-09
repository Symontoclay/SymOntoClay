/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public abstract class BaseComponent: BaseLoggedComponent, ISymOntoClayDisposable
    {
        protected BaseComponent(IEntityLogger logger)
            : base(logger)
        {
        }

        protected ComponentState _state = ComponentState.Created;
        protected readonly object _stateLockObj = new object();

        /// <inheritdoc/>
        public bool IsDisposed
        {
            get
            {
                lock (_stateLockObj)
                {
                    return _state == ComponentState.Disposed;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_stateLockObj)
            {
                if (_state == ComponentState.Disposed)
                {
                    return;
                }

                _state = ComponentState.Disposed;
            }

            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
        }
    }
}
