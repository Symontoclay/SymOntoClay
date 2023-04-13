/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class PlatformProcessInfo : BaseProcessInfo
    {
        public PlatformProcessInfo(CancellationTokenSource cancellationTokenSource, string endPointName, IReadOnlyList<int> devices, IReadOnlyList<string> friends)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _endPointName = endPointName;
            _devices = devices;
            _friends = friends;
        }

        public void SetTask(Task task)
        {
            _task = task;
        }

        /// <inheritdoc/>
        public override string EndPointName => _endPointName;

        /// <inheritdoc/>
        public override ProcessStatus Status
        {
            get
            {
                lock (_statusLockObj)
                {
                    return _status;
                }
            }

            set
            {
                lock (_statusLockObj)
                {
                    NSetStatus(value);
                }
            }
        }

        private void NSetStatus(ProcessStatus status)
        {
            if (_status == status)
            {
                return;
            }

            _status = status;

            switch (_status)
            {
                case ProcessStatus.Completed:
                    EmitOnComplete();
                    ProcessGeneralFinishStatuses();
                    break;

                case ProcessStatus.Canceled:
                case ProcessStatus.WeakCanceled:
                case ProcessStatus.Faulted:
                    ProcessGeneralFinishStatuses();
                    break;
            }
        }

        private void EmitOnFinish()
        {
            Task.Run(() => {
                InternalOnFinish?.Invoke(this);
            });
        }

        private void EmitOnComplete()
        {
            Task.Run(() => {
                InternalOnComplete?.Invoke(this);
            });
        }

        private void ProcessGeneralFinishStatuses()
        {
            EmitOnFinish();
            NCancelChildren();
        }

        /// <inheritdoc/>
        public override IReadOnlyList<int> Devices => _devices;

        /// <inheritdoc/>
        public override void Start()
        {
            lock (_statusLockObj)
            {
                _task.Start();

                _status = ProcessStatus.Running;
            }
        }

        /// <inheritdoc/>
        public override void Cancel()
        {
            lock (_statusLockObj)
            {
                if(IsFinished)
                {
                    return;
                }

                _status = ProcessStatus.Canceled;
                _cancellationTokenSource.Cancel();
                
                EmitOnFinish();

                NCancelChildren();
            }
        }

        /// <inheritdoc/>
        public override void WeakCancel()
        {
            lock (_statusLockObj)
            {
                if (IsFinished)
                {
                    return;
                }

                _status = ProcessStatus.WeakCanceled;
                _cancellationTokenSource.Cancel();

                EmitOnFinish();

                NCancelChildren();
            }
        }

        /// <inheritdoc/>
        public override event ProcessInfoEvent OnFinish
        {
            add
            {
                lock (_statusLockObj)
                {
                    InternalOnFinish += value;

                    if (IsFinished)
                    {
                        EmitOnFinish();
                    }
                }
            }

            remove
            {
                lock (_statusLockObj)
                {
                    InternalOnFinish -= value;
                }                
            }
        }

        private event ProcessInfoEvent InternalOnFinish;

        /// <inheritdoc/>
        public override event ProcessInfoEvent OnComplete
        {
            add
            {
                lock(_statusLockObj)
                {
                    InternalOnComplete += value;

                    if (_status == ProcessStatus.Completed)
                    {
                        EmitOnComplete();
                    }
                }
            }

            remove
            {
                lock (_statusLockObj)
                {
                    InternalOnComplete -= value;
                }                
            }
        }

        private event ProcessInfoEvent InternalOnComplete;

        /// <inheritdoc/>
        public override IReadOnlyList<string> Friends => _friends;

        /// <inheritdoc/>
        public override bool IsFriend(IProcessInfo other)
        {
            if ((!other.Friends.IsNullOrEmpty() && other.Friends.Any(p => p == EndPointName)) || (!Friends.IsNullOrEmpty() && Friends.Any(p => p == other.EndPointName)))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            lock (_statusLockObj)
            {
                _cancellationTokenSource.Cancel();
            }

            base.OnDisposed();
        }

        #region private fields
        private readonly string _endPointName;
        private ProcessStatus _status = ProcessStatus.Created;
        private readonly IReadOnlyList<int> _devices;
        private readonly IReadOnlyList<string> _friends;
        private Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource;
        #endregion
    }
}
