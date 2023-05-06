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

using Newtonsoft.Json.Linq;
using NLog;
using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core
{
    public abstract class BaseProcessInfo : IProcessInfo
    {
        protected static ILogger _logger = LogManager.GetCurrentClassLogger();

        protected BaseProcessInfo()
        {
            Id = NameHelper.GetNewEntityNameString();
        }

        /// <inheritdoc/>
        public string Id { get; private set; }

        /// <inheritdoc/>
        public abstract string EndPointName { get; }

        /// <inheritdoc/>
        public ProcessStatus Status 
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
                    if (_status == value)
                    {
                        return;
                    }

                    if (NIsFinished && value != ProcessStatus.WeakCanceled)
                    {
                        return;
                    }

                    _status = value;
                }

                ProcessSetStatus(value);
            }
        }

        /// <inheritdoc/>
        public bool IsFinished
        {
            get
            {
                lock (_statusLockObj)
                {
                    return NIsFinished;
                }
            }
        }

        /// <inheritdoc/>
        public void Start()
        {
            lock (_statusLockObj)
            {
                if (NIsFinished || _status == ProcessStatus.Running)
                {
                    return;
                }

                _status = ProcessStatus.Running;
            }

            ProcessPlatformStart();
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            lock (_statusLockObj)
            {
                if (NIsFinished)
                {
                    return;
                }

                _status = ProcessStatus.Canceled;
            }

            ProcessSetStatus(ProcessStatus.Canceled);
        }

        /// <inheritdoc/>
        public void WeakCancel()
        {
            lock (_statusLockObj)
            {
#if DEBUG
                _logger.Info($"WeakCancel GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

                if (NIsFinished)
                {
                    return;
                }

#if DEBUG
                _logger.Info($"WeakCancel NEXT GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

                _status = ProcessStatus.WeakCanceled;
            }

            ProcessSetStatus(ProcessStatus.WeakCanceled);
        }

        protected bool NIsFinished
        {
            get
            {
                var status = _status;
                return status == ProcessStatus.Completed || status == ProcessStatus.Canceled || status == ProcessStatus.WeakCanceled || status == ProcessStatus.Faulted;
            }
        }

        private void ProcessSetStatus(ProcessStatus status)
        {
#if DEBUG
            _logger.Info($"ProcessSetStatus status = {status}; GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

            switch (status)
            {
                case ProcessStatus.Completed:
                    EmitOnComplete();
                    ProcessGeneralFinishStatuses(ProcessStatus.WeakCanceled);
                    break;

                case ProcessStatus.WeakCanceled:
                    EmitOnWeakCanceled();
                    ProcessGeneralFinishStatuses(ProcessStatus.WeakCanceled);
                    ProcessPlatformCancelation();
                    break;

                case ProcessStatus.Canceled:
                case ProcessStatus.Faulted:
                    ProcessGeneralFinishStatuses(ProcessStatus.Canceled);
                    ProcessPlatformCancelation();
                    break;
            }
        }

        protected virtual void ProcessPlatformStart()
        {
        }

        protected virtual void ProcessPlatformCancelation()
        {
        }

        private void EmitOnFinish()
        {
            EmitOnFinishHandlers();

            InternalOnFinish?.Invoke(this);
        }

        private void EmitOnComplete()
        {
            EmitOnCompleteHandlers();

            InternalOnComplete?.Invoke(this);
        }

        private void EmitOnWeakCanceled()
        {
            EmitOnWeakCanceledHandlers();

            InternalOnWeakCanceled?.Invoke(this);
        }

        private void ProcessGeneralFinishStatuses(ProcessStatus status)
        {
#if DEBUG
            _logger.Info($"ProcessGeneralFinishStatuses status = {status} GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

            EmitOnFinish();
            NCancelChildren(status);
        }

        private void NCancelChildren(ProcessStatus status)
        {
#if DEBUG
            _logger.Info($"NCancelChildren status = {status} GetType().FullName = {GetType().FullName} ({GetHashCode()}); _childrenProcessInfoList.Count = {_childrenProcessInfoList.Count}");
#endif

            switch (status)
            {
                case ProcessStatus.WeakCanceled:
                    foreach (var child in _childrenProcessInfoList.ToList())
                    {
#if DEBUG
                        _logger.Info($"NCancelChildren child = {child.GetType().FullName}; GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

                        child.WeakCancel();

#if DEBUG
                        _logger.Info($"NCancelChildren NEXT child = {child.GetType().FullName}; GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif
                    }
                    break;

                case ProcessStatus.Canceled:
                    foreach (var child in _childrenProcessInfoList.ToList())
                    {
                        child.Cancel();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        /// <inheritdoc/>
        public event ProcessInfoEvent OnFinish
        {
            add
            {
                lock (_statusLockObj)
                {
                    InternalOnFinish += value;

                    CheckOnFinishStatus();
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
        public event ProcessInfoEvent OnComplete
        {
            add
            {
                lock (_statusLockObj)
                {
                    InternalOnComplete += value;

                    CheckOnCompleteStatus();
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
        public event ProcessInfoEvent OnWeakCanceled
        {
            add
            {
                lock (_statusLockObj)
                {
                    InternalOnWeakCanceled += value;

                    CheckOnWeakCanceledStatus();
                }
            }

            remove
            {
                lock (_statusLockObj)
                {
                    InternalOnWeakCanceled -= value;
                }
            }
        }

        private event ProcessInfoEvent InternalOnWeakCanceled;

        protected void CheckOnFinishStatus()
        {
            if (NIsFinished)
            {
                EmitOnFinish();
            }
        }

        protected void CheckOnCompleteStatus()
        {
            if (_status == ProcessStatus.Completed)
            {
                EmitOnComplete();
            }
        }

        protected void CheckOnWeakCanceledStatus()
        {
            if (_status == ProcessStatus.WeakCanceled)
            {
                EmitOnWeakCanceled();
            }
        }

        /// <inheritdoc/>
        public abstract IReadOnlyList<int> Devices { get; }

        /// <inheritdoc/>
        public float Priority { get; set; } = PrioritiesOfProcesses.Normal;

        /// <inheritdoc/>
        public float GlobalPriority
        {
            get
            {
                if(ParentProcessInfo == null)
                {
                    return Priority;
                }

                return Priority * ParentProcessInfo.GlobalPriority;
            }
        }

        /// <inheritdoc/>
        public abstract IReadOnlyList<string> Friends { get; }

        /// <inheritdoc/>
        public abstract bool IsFriend(IProcessInfo other);

        /// <inheritdoc/>
        public IProcessInfo ParentProcessInfo
        { 
            get
            {
                lock (_parentAndChildrenLockObj)
                {
                    return _parentProcessInfo;
                }
            }

            set
            {
                lock (_parentAndChildrenLockObj)
                {
                    if(_parentProcessInfo == value)
                    {
                        return;
                    }

                    if (_parentProcessInfo == this)
                    {
                        return;
                    }

                    if (_parentProcessInfo != null)
                    {
                        _parentProcessInfo.RemoveChild(this);
                    }

                    _parentProcessInfo = value;

                    if (_parentProcessInfo != null)
                    {
                        _parentProcessInfo.AddChild(this);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IProcessInfo> GetChildrenProcessInfoList 
        { 
            get
            {
                lock(_parentAndChildrenLockObj)
                {
                    return _childrenProcessInfoList.ToList();
                }
            }
        }

        /// <inheritdoc/>
        public void AddChild(IProcessInfo processInfo)
        {
            lock (_parentAndChildrenLockObj)
            {
#if DEBUG
                _logger.Info($"AddChild processInfo = {processInfo.GetType().FullName} ({processInfo.GetHashCode()}); GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

                if (processInfo == this)
                {
                    return;
                }

                if (processInfo == null)
                {
                    return;
                }

                if (_childrenProcessInfoList.Contains(processInfo))
                {
                    return;
                }

#if DEBUG
                _logger.Info($"AddChild NEXT processInfo = {processInfo.GetType().FullName} ({processInfo.GetHashCode()}); GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

                _childrenProcessInfoList.Add(processInfo);

                processInfo.OnFinish += ProcessInfo_OnFinish;

                if(processInfo.ParentProcessInfo != this)
                {
                    processInfo.ParentProcessInfo = this;
                }
            }
        }

        private void ProcessInfo_OnFinish(IProcessInfo processInfo)
        {
            lock (_parentAndChildrenLockObj)
            {
                NRemoveChild(processInfo);
            }
        }

        /// <inheritdoc/>
        public void RemoveChild(IProcessInfo processInfo)
        {
            lock (_parentAndChildrenLockObj)
            {
                NRemoveChild(processInfo);
            }
        }

        private void NRemoveChild(IProcessInfo processInfo)
        {
#if DEBUG
            _logger.Info($"NRemoveChild processInfo = {processInfo.GetType().FullName} ({processInfo.GetHashCode()}); GetType().FullName = {GetType().FullName} ({GetHashCode()})");
#endif

            if (!_childrenProcessInfoList.Contains(processInfo))
            {
                return;
            }

            _childrenProcessInfoList.Remove(processInfo);

            processInfo.OnFinish -= ProcessInfo_OnFinish;

            if (processInfo.ParentProcessInfo == this)
            {
                processInfo.ParentProcessInfo = null;
            }
        }

        /// <inheritdoc/>
        public void AddOnFinishHandler(IProcessInfoEventHandler handler)
        {
            lock (_parentAndChildrenLockObj)
            {
                if(handler == null)
                {
                    return;
                }

                if(_onFinishHandlersList.Contains(handler))
                {
                    return;
                }

                _onFinishHandlersList.Add(handler);

                if(handler.ProcessInfo != this)
                {
                    handler.ProcessInfo = this;
                }

                CheckOnFinishStatus();
            }
        }

        /// <inheritdoc/>
        public void RemoveOnFinishHandler(IProcessInfoEventHandler handler)
        {
            lock (_parentAndChildrenLockObj)
            {
                if (!_onFinishHandlersList.Contains(handler))
                {
                    return;
                }

                _onFinishHandlersList.Remove(handler);

                if (handler.ProcessInfo == this)
                {
                    handler.ProcessInfo = null;
                }
            }
        }

        protected void EmitOnFinishHandlers()
        {
            lock (_parentAndChildrenLockObj)
            {
                foreach (var item in _onFinishHandlersList)
                {
                    item.Run();
                }
            }
        }

        /// <inheritdoc/>
        public void AddOnCompleteHandler(IProcessInfoEventHandler handler)
        {
            lock (_parentAndChildrenLockObj)
            {
                if (handler == null)
                {
                    return;
                }

                if (_onCompleteHandlersList.Contains(handler))
                {
                    return;
                }

                _onCompleteHandlersList.Add(handler);

                if (handler.ProcessInfo != this)
                {
                    handler.ProcessInfo = this;
                }

                CheckOnCompleteStatus();
            }
        }

        /// <inheritdoc/>
        public void RemoveOnCompleteHandler(IProcessInfoEventHandler handler)
        {
            lock (_parentAndChildrenLockObj)
            {
                if (!_onCompleteHandlersList.Contains(handler))
                {
                    return;
                }

                _onCompleteHandlersList.Remove(handler);

                if (handler.ProcessInfo == this)
                {
                    handler.ProcessInfo = null;
                }
            }
        }

        protected void EmitOnCompleteHandlers()
        {
            lock (_parentAndChildrenLockObj)
            {
                foreach (var item in _onCompleteHandlersList)
                {
                    item.Run();
                }
            }
        }

        /// <inheritdoc/>
        public void AddOnWeakCanceledHandler(IProcessInfoEventHandler handler)
        {
            lock (_parentAndChildrenLockObj)
            {
                if (handler == null)
                {
                    return;
                }

                if (_onWeakCanceledHandlersList.Contains(handler))
                {
                    return;
                }

                _onWeakCanceledHandlersList.Add(handler);

                if (handler.ProcessInfo != this)
                {
                    handler.ProcessInfo = this;
                }

                CheckOnWeakCanceledStatus();
            }
        }

        /// <inheritdoc/>
        public void RemoveOnWeakCanceledHandler(IProcessInfoEventHandler handler)
        {
            lock (_parentAndChildrenLockObj)
            {
                if (!_onWeakCanceledHandlersList.Contains(handler))
                {
                    return;
                }

                _onWeakCanceledHandlersList.Remove(handler);

                if (handler.ProcessInfo == this)
                {
                    handler.ProcessInfo = null;
                }
            }
        }

        protected void EmitOnWeakCanceledHandlers()
        {
            lock (_parentAndChildrenLockObj)
            {
                foreach (var item in _onWeakCanceledHandlersList)
                {
                    item.Run();
                }
            }
        }

        #region private fields
        protected object _statusLockObj = new object();
        protected readonly object _parentAndChildrenLockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
        private IProcessInfo _parentProcessInfo;
        private List<IProcessInfo> _childrenProcessInfoList = new List<IProcessInfo>();
        private List<IProcessInfoEventHandler> _onFinishHandlersList = new List<IProcessInfoEventHandler>();
        private List<IProcessInfoEventHandler> _onCompleteHandlersList = new List<IProcessInfoEventHandler>();
        private List<IProcessInfoEventHandler> _onWeakCanceledHandlersList = new List<IProcessInfoEventHandler>();
        #endregion

        private bool _isDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            lock(_parentAndChildrenLockObj)
            {
                if(_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
            }

            OnDisposed();
        }

        protected virtual void OnDisposed()
        {
            foreach(var child in _childrenProcessInfoList.ToList())
            {
                child.Dispose();
            }

            _childrenProcessInfoList.Clear();

            foreach(var item in _onFinishHandlersList)
            {
                item.Dispose();
            }

            _onFinishHandlersList.Clear();

            foreach (var item in _onCompleteHandlersList)
            {
                item.Dispose();
            }

            _onCompleteHandlersList.Clear();

            foreach (var item in _onWeakCanceledHandlersList)
            {
                item.Dispose();
            }

            _onWeakCanceledHandlersList.Clear();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(EndPointName)} = {EndPointName}");
            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.AppendLine($"{spaces}{nameof(IsFinished)} = {IsFinished}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintPODList(n, nameof(Friends), Friends);

            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");
            sb.AppendLine($"{spaces}{nameof(GlobalPriority)} = {GlobalPriority}");

            sb.PrintBriefObjProp(n, nameof(ParentProcessInfo), ParentProcessInfo);
            sb.PrintObjListProp(n, "Children", GetChildrenProcessInfoList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(EndPointName)} = {EndPointName}");
            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.AppendLine($"{spaces}{nameof(IsFinished)} = {IsFinished}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintPODList(n, nameof(Friends), Friends);

            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");
            sb.AppendLine($"{spaces}{nameof(GlobalPriority)} = {GlobalPriority}");

            sb.PrintBriefObjProp(n, nameof(ParentProcessInfo), ParentProcessInfo);
            sb.PrintShortObjListProp(n, "Children", GetChildrenProcessInfoList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(EndPointName)} = {EndPointName}");
            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.AppendLine($"{spaces}{nameof(IsFinished)} = {IsFinished}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintPODList(n, nameof(Friends), Friends);

            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");
            sb.AppendLine($"{spaces}{nameof(GlobalPriority)} = {GlobalPriority}");

            sb.PrintExisting(n, nameof(ParentProcessInfo), ParentProcessInfo);
            sb.PrintExistingList(n, "Children", GetChildrenProcessInfoList);

            return sb.ToString();
        }
    }
}
