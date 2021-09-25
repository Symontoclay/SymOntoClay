/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            lock (_lockObj)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        #region private fields
        private readonly object _lockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
        private readonly IReadOnlyList<int> _devices;
        private Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource;
        #endregion
    }
}
