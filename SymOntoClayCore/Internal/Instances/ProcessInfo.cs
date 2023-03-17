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

using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ProcessInfo: BaseProcessInfo
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public override ProcessStatus Status 
        { 
            get
            {
                lock(_lockObj)
                {
                    return _status;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    if(_status == value)
                    {
                        return;
                    }

                    _status = value;

                    switch(_status)
                    {
                        case ProcessStatus.Completed:
                            Task.Run(() => {
                                OnComplete?.Invoke(this);
                            });
                            ProcessGeneralFinishStatuses();
                            break;

                        case ProcessStatus.Canceled:
                        case ProcessStatus.Faulted:
                            ProcessGeneralFinishStatuses();
                            break;
                    }
                }
            }
        }

        private void ProcessGeneralFinishStatuses()
        {
            Task.Run(() => {
                OnFinish?.Invoke(this);
            });
            CancelChildren();
        }

        /// <inheritdoc/>
        public override string EndPointName => string.Empty;

        public CodeFrame CodeFrame { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<int> Devices => _devices;

        /// <inheritdoc/>
        public override event ProcessInfoEvent OnFinish;

        /// <inheritdoc/>
        public override event ProcessInfoEvent OnComplete;

        /// <inheritdoc/>
        public override void Start()
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Cancel()
        {
#if DEBUG
            //_gbcLogger.Info($"Cancel Id = {Id}");
#endif

            Status = ProcessStatus.Canceled;

            base.Cancel();
        }

        /// <inheritdoc/>
        public override IReadOnlyList<string> Friends => _friends;

        /// <inheritdoc/>
        public override bool IsFriend(IProcessInfo other)
        {
            return false;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        #region private fields
        private readonly object _lockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
        private readonly List<int> _devices = new List<int>();
        private readonly List<string> _friends = new List<string>();
        #endregion

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(CodeFrame), CodeFrame);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(CodeFrame), CodeFrame);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(CodeFrame), CodeFrame);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
