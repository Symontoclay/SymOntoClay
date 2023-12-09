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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.NLog;
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
        protected static IMonitorLogger _logger = MonitorLoggerNLogImpementation.Instance;
        
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

                ProcessSetStatus(_logger, value);
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
        public void Start(IMonitorLogger logger)
        {
            lock (_statusLockObj)
            {
                if (NIsFinished || _status == ProcessStatus.Running)
                {
                    return;
                }

                _status = ProcessStatus.Running;
            }

            ProcessPlatformStart(logger);
        }

        /// <inheritdoc/>
        public void Cancel(IMonitorLogger logger)
        {
            lock (_statusLockObj)
            {
                if (NIsFinished)
                {
                    return;
                }

                _status = ProcessStatus.Canceled;
            }

            ProcessSetStatus(logger, ProcessStatus.Canceled);
        }

        /// <inheritdoc/>
        public void WeakCancel(IMonitorLogger logger)
        {
            lock (_statusLockObj)
            {
                if (NIsFinished)
                {
                    return;
                }

                _status = ProcessStatus.WeakCanceled;
            }

            ProcessSetStatus(logger, ProcessStatus.WeakCanceled);
        }

        protected bool NIsFinished
        {
            get
            {
                var status = _status;
                return status == ProcessStatus.Completed || status == ProcessStatus.Canceled || status == ProcessStatus.WeakCanceled || status == ProcessStatus.Faulted;
            }
        }

        private void ProcessSetStatus(IMonitorLogger logger, ProcessStatus status)
        {
            switch (status)
            {
                case ProcessStatus.Completed:
                    EmitOnComplete(logger);
                    ProcessGeneralFinishStatuses(logger, ProcessStatus.WeakCanceled);
                    break;

                case ProcessStatus.WeakCanceled:
                    EmitOnWeakCanceled(logger);
                    ProcessGeneralFinishStatuses(logger, ProcessStatus.WeakCanceled);
                    ProcessPlatformCancelation(logger);
                    break;

                case ProcessStatus.Canceled:
                case ProcessStatus.Faulted:
                    ProcessGeneralFinishStatuses(logger, ProcessStatus.Canceled);
                    ProcessPlatformCancelation(logger);
                    break;
            }
        }

        protected virtual void ProcessPlatformStart(IMonitorLogger logger)
        {
        }

        protected virtual void ProcessPlatformCancelation(IMonitorLogger logger)
        {
        }

        private void EmitOnFinish(IMonitorLogger logger)
        {
            EmitOnFinishHandlers(logger);

            InternalOnFinish?.Invoke(this);
        }

        private void EmitOnComplete(IMonitorLogger logger)
        {
            EmitOnCompleteHandlers(logger);

            InternalOnComplete?.Invoke(this);
        }

        private void EmitOnWeakCanceled(IMonitorLogger logger)
        {
            EmitOnWeakCanceledHandlers(logger);

            InternalOnWeakCanceled?.Invoke(this);
        }

        private void ProcessGeneralFinishStatuses(IMonitorLogger logger, ProcessStatus status)
        {
            EmitOnFinish(logger);
            NCancelChildren(logger, status);
        }

        private void NCancelChildren(IMonitorLogger logger, ProcessStatus status)
        {
            switch (status)
            {
                case ProcessStatus.WeakCanceled:
                    foreach (var child in _childrenProcessInfoList.ToList())
                    {
                        child.WeakCancel(logger);
                    }
                    break;

                case ProcessStatus.Canceled:
                    foreach (var child in _childrenProcessInfoList.ToList())
                    {
                        child.Cancel(logger);
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

                    CheckOnFinishStatus(_logger);
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

                    CheckOnCompleteStatus(_logger);
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

                    CheckOnWeakCanceledStatus(_logger);
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

        protected void CheckOnFinishStatus(IMonitorLogger logger)
        {
            if (NIsFinished)
            {
                EmitOnFinish(logger);
            }
        }

        protected void CheckOnCompleteStatus(IMonitorLogger logger)
        {
            if (_status == ProcessStatus.Completed)
            {
                EmitOnComplete(logger);
            }
        }

        protected void CheckOnWeakCanceledStatus(IMonitorLogger logger)
        {
            if (_status == ProcessStatus.WeakCanceled)
            {
                EmitOnWeakCanceled(logger);
            }
        }

        /// <inheritdoc/>
        public abstract IReadOnlyList<int> Devices { get; }

        /// <inheritdoc/>
        public float Priority { get; set; } = PrioritiesOfProcesses.Normal;

        /// <inheritdoc/>
        public abstract IReadOnlyList<string> Friends { get; }

        /// <inheritdoc/>
        public abstract bool IsFriend(IMonitorLogger logger, IProcessInfo other);

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
                    if (_parentProcessInfo == value)
                    {
                        return;
                    }

                    if (_parentProcessInfo == this)
                    {
                        return;
                    }

                    if (_parentProcessInfo != null)
                    {
                        _parentProcessInfo.RemoveChild(_logger, this);
                    }

                    _parentProcessInfo = value;

                    if (_parentProcessInfo != null)
                    {
                        _parentProcessInfo.AddChild(_logger, this);
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
        public void AddChild(IMonitorLogger logger, IProcessInfo processInfo)
        {
            lock (_parentAndChildrenLockObj)
            {
                if (processInfo == this)
                {
                    return;
                }

                if (processInfo == null)
                {
                    return;
                }

                if(_removedChildrenProcessInfoList.Contains(processInfo))
                {
                    return;
                }

                if (_childrenProcessInfoList.Contains(processInfo))
                {
                    return;
                }

                _childrenProcessInfoList.Add(processInfo);

                processInfo.OnFinish += ProcessInfo_OnFinish;

                if (processInfo.ParentProcessInfo != this)
                {
                    processInfo.ParentProcessInfo = this;
                }
            }
        }

        private void ProcessInfo_OnFinish(IProcessInfo processInfo)
        {
            lock (_parentAndChildrenLockObj)
            {
                NRemoveChild(_logger, processInfo);
            }
        }

        /// <inheritdoc/>
        public void RemoveChild(IMonitorLogger logger, IProcessInfo processInfo)
        {
            lock (_parentAndChildrenLockObj)
            {
                NRemoveChild(_logger, processInfo);
            }
        }

        private void NRemoveChild(IMonitorLogger logger, IProcessInfo processInfo)
        {
            if (!_childrenProcessInfoList.Contains(processInfo))
            {
                return;
            }

            _childrenProcessInfoList.Remove(processInfo);
            _removedChildrenProcessInfoList.Add(processInfo);

            processInfo.OnFinish -= ProcessInfo_OnFinish;

            if (processInfo.ParentProcessInfo == this)
            {
                processInfo.ParentProcessInfo = null;
            }
        }

        /// <inheritdoc/>
        public void AddOnFinishHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
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

                CheckOnFinishStatus(logger);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnFinishHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
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

        protected void EmitOnFinishHandlers(IMonitorLogger logger)
        {
            lock (_parentAndChildrenLockObj)
            {
                foreach (var item in _onFinishHandlersList)
                {
                    item.Run(logger);
                }
            }
        }

        /// <inheritdoc/>
        public void AddOnCompleteHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
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

                CheckOnCompleteStatus(logger);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnCompleteHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
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

        protected void EmitOnCompleteHandlers(IMonitorLogger logger)
        {
            lock (_parentAndChildrenLockObj)
            {
                foreach (var item in _onCompleteHandlersList)
                {
                    item.Run(logger);
                }
            }
        }

        /// <inheritdoc/>
        public void AddOnWeakCanceledHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
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

                CheckOnWeakCanceledStatus(logger);
            }
        }

        /// <inheritdoc/>
        public void RemoveOnWeakCanceledHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
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

        protected void EmitOnWeakCanceledHandlers(IMonitorLogger logger)
        {
            lock (_parentAndChildrenLockObj)
            {
                foreach (var item in _onWeakCanceledHandlersList)
                {
                    item.Run(logger);
                }
            }
        }

        #region private fields
        protected object _statusLockObj = new object();
        protected readonly object _parentAndChildrenLockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
        private IProcessInfo _parentProcessInfo;
        private List<IProcessInfo> _childrenProcessInfoList = new List<IProcessInfo>();
        private List<IProcessInfo> _removedChildrenProcessInfoList = new List<IProcessInfo>();
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

            sb.PrintExisting(n, nameof(ParentProcessInfo), ParentProcessInfo);
            sb.PrintExistingList(n, "Children", GetChildrenProcessInfoList);

            return sb.ToString();
        }
        
        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(opt);
        }

        /// <inheritdoc/>
        public abstract string ToHumanizedString(DebugHelperOptions options);

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedLabel(opt);
        }

        /// <inheritdoc/>
        public abstract string ToHumanizedLabel(DebugHelperOptions options);

        /// <inheritdoc/>
        public abstract string ToHumanizedString(IMonitorLogger logger);

        /// <inheritdoc/>
        public abstract MonitoredHumanizedLabel ToLabel(IMonitorLogger logger);

        /// <inheritdoc/>
        public IReadOnlyList<MonitoredHumanizedLabel> ToChainOfProcessInfoLabels(IMonitorLogger logger)
        {
            var result = new List<MonitoredHumanizedLabel>();
            var usedProcessInfo = new List<IProcessInfo>();

            CollectChainOfProcessInfoLabels(logger, result, usedProcessInfo);

            result.Reverse();

            return result;
        }

        /// <inheritdoc/>
        public void CollectChainOfProcessInfoLabels(IMonitorLogger logger, IList<MonitoredHumanizedLabel> result, IList<IProcessInfo> usedProcessInfo)
        {
            if(usedProcessInfo.Contains(this))
            {
                return;
            }

            usedProcessInfo.Add(this);

            result.Add(ToLabel(logger));

            if(ParentProcessInfo != null)
            {
                ParentProcessInfo.CollectChainOfProcessInfoLabels(logger, result, usedProcessInfo);
            }
        }

        /*
        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            return $"proc: {Id} ({Status})";
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException();
        }
        */
    }
}
