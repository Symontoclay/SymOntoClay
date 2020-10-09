/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class PlatformProcessInfo : BaseProcessInfo
    {
        public PlatformProcessInfo(CancellationTokenSource cancellationTokenSource, IReadOnlyList<int> devices)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _devices = devices;
        }

        public void SetTask(Task task)
        {
            _task = task;
        }

        /// <inheritdoc/>
        public override ProcessStatus Status
        {
            get
            {
                lock (_lockObj)
                {
                    return _status;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    if (_status == value)
                    {
                        return;
                    }

                    _status = value;

                    switch(_status)
                    {
                        case ProcessStatus.Canceled:
                        case ProcessStatus.Completed:
                        case ProcessStatus.Faulted:
                            Task.Run(() => {
                                OnFinish?.Invoke(this);
                            });
                            break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override IReadOnlyList<int> Devices => _devices;

        /// <inheritdoc/>
        public override void Start()
        {
            lock (_lockObj)
            {
                _task.Start();
                _status = ProcessStatus.Running;
            }
        }

        /// <inheritdoc/>
        public override void Cancel()
        {
            lock (_lockObj)
            {
                _cancellationTokenSource.Cancel();
                _status = ProcessStatus.Canceled;

                OnFinish?.Invoke(this);
            }
        }

        /// <inheritdoc/>
        public override event ProcessInfoEvent OnFinish;

        #region private fields
        private readonly object _lockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
        private readonly IReadOnlyList<int> _devices;
        private Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource;
        #endregion
    }
}
