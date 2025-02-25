/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core
{
    public abstract class BaseProcessInfo : IProcessInfo,
        IOnFinishProcessInfoHandler
    {
        protected static IMonitorLogger _logger = MonitorLoggerNLogImpementation.Instance;
        
        protected BaseProcessInfo(CancellationToken cancellationToken, ICustomThreadPool threadPool, IActiveObjectContext activeObjectContext)
        {
            _cancellationToken = cancellationToken;
            _threadPool = threadPool;
            _activeObjectContext = activeObjectContext;
            _serializationAnchor = new SerializationAnchor();

            Id = NameHelper.GetNewEntityNameString();
        }

        protected readonly CancellationToken _cancellationToken;
        protected readonly ICustomThreadPool _threadPool;
        protected IActiveObjectContext _activeObjectContext;
        protected SerializationAnchor _serializationAnchor;

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
        }

        /// <inheritdoc/>
        public void SetStatus(IMonitorLogger logger, string messagePointId, ProcessStatus status)
        {
            var prevStatus = _status;

#if DEBUG
            //logger.Info("C6214028-0F57-4737-BDE5-9B15803E41AC", $"status = {status};prevStatus = {prevStatus};{ToHumanizedLabel()}");
#endif

            lock (_statusLockObj)
            {
#if DEBUG
                //logger.Info("589BA5A4-25D5-45A4-B6AD-2303154435F4", $"A;{ToHumanizedLabel()}");
#endif

                if (_status == status)
                {
#if DEBUG
                    //logger.Info("04F8EF32-7543-49FB-BA03-9803245DD3D1", $"B;{ToHumanizedLabel()}");
#endif

                    return;
                }

#if DEBUG
                //logger.Info("75FD0D05-6208-4EC0-8FCD-864D3AEA68AE", $"C;{ToHumanizedLabel()}");
#endif

                if (NIsFinished(logger) && status != ProcessStatus.WeakCanceled)
                {
#if DEBUG
                    //logger.Info("D1E26BCE-1076-410D-A350-2189CE993B12", $"D;{ToHumanizedLabel()}");
#endif

                    return;
                }

                prevStatus = _status;

                _status = status;
            }

#if DEBUG
            //logger.Info("40E621E1-49BD-4F9B-B8CB-C36699D70524", $"E;{ToHumanizedLabel()}");
#endif

            logger.SetProcessInfoStatus(messagePointId, Id, ToLabel(logger), status, prevStatus, null, null);

            ProcessSetStatus(logger, status, callMethodId: string.Empty);
        }

        /// <inheritdoc/>
        public bool IsFinished(IMonitorLogger logger)
        {
            lock (_statusLockObj)
            {
                return NIsFinished(logger);
            }
        }

        /// <inheritdoc/>
        public void Start(IMonitorLogger logger, string messagePointId)
        {
#if DEBUG
            //logger.Info("0D259105-3AC7-4973-96A4-2AF245D343C3", $"Begin;messagePointId = {messagePointId};{ToHumanizedLabel()}");
#endif

            lock (_statusLockObj)
            {
#if DEBUG
                //logger.Info("4ABAF301-7B54-4896-9436-4C39FB12226E", $"_status = {_status};messagePointId = {messagePointId};{ToHumanizedLabel()}");
#endif

                if (NIsFinished(logger) || _status == ProcessStatus.Running)
                {
#if DEBUG
                    //logger.Info("A01EADE3-B58F-45A5-922A-A5ACD3DF2C6A", $"ret!!!;messagePointId = {messagePointId};{ToHumanizedLabel()}");
#endif

                    return;
                }

#if DEBUG
                //logger.Info("65078904-72F8-45F3-AF73-BE63EFD1CA3D", $"Before;messagePointId = {messagePointId};{ToHumanizedLabel()}");
#endif

                _status = ProcessStatus.Running;
            }

            logger.StartProcessInfo(messagePointId, Id, ToLabel(logger));

#if DEBUG
            //logger.Info("02CEB02F-F4E0-4AFF-96FB-20493F10EC8C", $"After _status = {_status};messagePointId = {messagePointId};{ToHumanizedLabel()}");
#endif

            ProcessPlatformStart(logger);

#if DEBUG
            //logger.Info("1EBCDD00-707D-4988-9BFF-6E5E97D050E9", $"End ;messagePointId = {messagePointId};{ToHumanizedLabel()}");
#endif
        }

        /// <inheritdoc/>
        public void Cancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, Changer changer = null, string callMethodId = "")
        {
            Cancel(logger, messagePointId, reasonOfChangeStatus, changer == null ? null : new List<Changer> { changer }, callMethodId);
        }

        /// <inheritdoc/>
        public void Cancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, List<Changer> changers, string callMethodId = "")
        {
            lock (_statusLockObj)
            {
                if (NIsFinished(logger))
                {
                    return;
                }

                _status = ProcessStatus.Canceled;
            }

            logger.CancelProcessInfo(messagePointId, Id, ToLabel(logger), reasonOfChangeStatus, changers, callMethodId);

            ProcessSetStatus(logger, ProcessStatus.Canceled, callMethodId);
        }

        /// <inheritdoc/>
        public void WeakCancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, Changer changer = null, string callMethodId = "")
        {
            WeakCancel(logger, messagePointId, reasonOfChangeStatus, changer == null ? null : new List<Changer> { changer }, callMethodId);
        }

        /// <inheritdoc/>
        public void WeakCancel(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, List<Changer> changers, string callMethodId = "")
        {
            lock (_statusLockObj)
            {
                if (NIsFinished(logger))
                {
                    return;
                }

                _status = ProcessStatus.WeakCanceled;
            }

            logger.WeakCancelProcessInfo(messagePointId, Id, ToLabel(logger), reasonOfChangeStatus, changers, callMethodId);

            ProcessSetStatus(logger, ProcessStatus.WeakCanceled, callMethodId);
        }

        protected bool NIsFinished(IMonitorLogger logger)
        {
            var status = _status;

#if DEBUG
            //logger?.Info("94B8136D-0648-41A0-90E9-856B2F557BE6", $"status = {status};{ToHumanizedLabel()}");
#endif

            return status == ProcessStatus.Completed || status == ProcessStatus.Canceled || status == ProcessStatus.WeakCanceled || status == ProcessStatus.Faulted;
        }

        private void ProcessSetStatus(IMonitorLogger logger, ProcessStatus status, string callMethodId)
        {
            switch (status)
            {
                case ProcessStatus.Completed:
                    EmitOnComplete(logger);
                    ProcessGeneralFinishStatuses(logger, ProcessStatus.WeakCanceled, callMethodId);
                    break;

                case ProcessStatus.WeakCanceled:
                    EmitOnWeakCanceled(logger);
                    ProcessGeneralFinishStatuses(logger, ProcessStatus.WeakCanceled, callMethodId);
                    ProcessPlatformCancelation(logger);
                    break;

                case ProcessStatus.Canceled:
                case ProcessStatus.Faulted:
                    ProcessGeneralFinishStatuses(logger, ProcessStatus.Canceled, callMethodId);
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
            lock (_parentAndChildrenLockObj)
            {
#if DEBUG
                //logger.Info("D85A951B-E74F-42CA-B7AF-1C8E15677C49", $"_onFinishHandlersList.Count = {_onFinishHandlersList.Count}");
#endif

                foreach (var item in _onFinishHandlersList)
                {
                    item.Run(logger);
                }
            }

            EmitOnFinishHandlers(this);
        }

        private void EmitOnComplete(IMonitorLogger logger)
        {
            lock (_parentAndChildrenLockObj)
            {
#if DEBUG
                //logger.Info("3758404B-0C33-4D48-BE14-0D123BA961C4", $"_onCompleteHandlersList.Count = {_onCompleteHandlersList.Count}");
#endif

                foreach (var item in _onCompleteHandlersList)
                {
                    item.Run(logger);
                }
            }

            EmitOnCompleteHandlers(this);
        }

        private void EmitOnWeakCanceled(IMonitorLogger logger)
        {
            lock (_parentAndChildrenLockObj)
            {
#if DEBUG
                //logger.Info("C77B12A1-0A22-41F5-BC6A-50A292DEB768", $"_onWeakCanceledHandlersList.Count = {_onWeakCanceledHandlersList.Count}");
#endif

                foreach (var item in _onWeakCanceledHandlersList)
                {
                    item.Run(logger);
                }
            }

            EmitOnWeakCanceledHandlers(this);
        }

        private void ProcessGeneralFinishStatuses(IMonitorLogger logger, ProcessStatus status, string callMethodId)
        {
            EmitOnFinish(logger);
            NCancelChildren(logger, status, callMethodId);
        }

        private void NCancelChildren(IMonitorLogger logger, ProcessStatus status, string callMethodId)
        {
            var changer = new Changer(KindOfChanger.ProcessInfo, Id);

            switch (status)
            {
                case ProcessStatus.WeakCanceled:
                    foreach (var child in _childrenProcessInfoList.ToList())
                    {
                        child.WeakCancel(logger, "256BA91B-55AE-4ABB-BCE6-44CE5CFD5A2F", ReasonOfChangeStatus.ByParentProcess, changer, callMethodId);
                    }
                    break;

                case ProcessStatus.Canceled:
                    foreach (var child in _childrenProcessInfoList.ToList())
                    {
                        child.Cancel(logger, "B710370A-6267-43F4-89FA-EF73F86A576E", ReasonOfChangeStatus.ByParentProcess, changer, callMethodId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        /// <inheritdoc/>
        public void AddOnFinishHandler(IOnFinishProcessInfoHandler handler)
        {
            lock (_onFinishHandlersLockObj)
            {
                if (_onFinishHandlers.Contains(handler))
                {
                    return;
                }

                _onFinishHandlers.Add(handler);

                if (NIsFinished(_logger))
                {
                    handler.Invoke(this);
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveOnFinishHandler(IOnFinishProcessInfoHandler handler)
        {
            lock (_onFinishHandlersLockObj)
            {
                if (_onFinishHandlers.Contains(handler))
                {
                    _onFinishHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnFinishHandlers(IProcessInfo sender)
        {
            lock (_onFinishHandlersLockObj)
            {
                foreach (var handler in _onFinishHandlers)
                {
                    handler.Invoke(sender);
                }
            }
        }

        private object _onFinishHandlersLockObj = new object();
        private List<IOnFinishProcessInfoHandler> _onFinishHandlers = new List<IOnFinishProcessInfoHandler>();

        /// <inheritdoc/>
        public void AddOnCompleteHandler(IOnCompleteProcessInfoHandler handler)
        {
            lock (_onCompleteHandlersLockObj)
            {
                if (_onCompleteHandlers.Contains(handler))
                {
                    return;
                }

                _onCompleteHandlers.Add(handler);

                if (_status == ProcessStatus.Completed)
                {
                    handler.Invoke(this);
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveOnCompleteHandler(IOnCompleteProcessInfoHandler handler)
        {
            lock (_onCompleteHandlersLockObj)
            {
                if (_onCompleteHandlers.Contains(handler))
                {
                    _onCompleteHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnCompleteHandlers(IProcessInfo sender)
        {
            lock (_onCompleteHandlersLockObj)
            {
                foreach (var handler in _onCompleteHandlers)
                {
                    handler.Invoke(sender);
                }
            }
        }

        private object _onCompleteHandlersLockObj = new object();
        private List<IOnCompleteProcessInfoHandler> _onCompleteHandlers = new List<IOnCompleteProcessInfoHandler>();

        /// <inheritdoc/>
        public void AddOnWeakCanceledHandler(IOnWeakCanceledProcessInfoHandler handler)
        {
            lock (_onWeakCanceledHandlersLockObj)
            {
                if (_onWeakCanceledHandlers.Contains(handler))
                {
                    return;
                }

                _onWeakCanceledHandlers.Add(handler);

                if (_status == ProcessStatus.WeakCanceled)
                {
                    handler.Invoke(this);
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveOnWeakCanceledHandler(IOnWeakCanceledProcessInfoHandler handler)
        {
            lock (_onWeakCanceledHandlersLockObj)
            {
                if (_onWeakCanceledHandlers.Contains(handler))
                {
                    _onWeakCanceledHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnWeakCanceledHandlers(IProcessInfo sender)
        {
            lock (_onWeakCanceledHandlersLockObj)
            {
                foreach (var handler in _onWeakCanceledHandlers)
                {
                    handler.Invoke(sender);
                }
            }
        }

        private object _onWeakCanceledHandlersLockObj = new object();
        private List<IOnWeakCanceledProcessInfoHandler> _onWeakCanceledHandlers = new List<IOnWeakCanceledProcessInfoHandler>();

        protected void CheckOnFinishStatus(IMonitorLogger logger)
        {
            if (NIsFinished(logger))
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

                processInfo.AddOnFinishHandler(this);

                if (processInfo.ParentProcessInfo != this)
                {
                    processInfo.ParentProcessInfo = this;
                }
            }
        }

        void IOnFinishProcessInfoHandler.Invoke(IProcessInfo sender)
        {
            LoggedFunctorWithoutResult<BaseProcessInfo, IProcessInfo>.Run(_logger, "54EEC6FF-3764-4F05-A28A-158B48FB2309", this, sender,
                (IMonitorLogger loggerValue, BaseProcessInfo instanceValue, IProcessInfo senderValue) => {
                    instanceValue.NProcessInfoOnFinish(senderValue);
                },
                _activeObjectContext, _threadPool, _serializationAnchor);
        }

        public void NProcessInfoOnFinish(IProcessInfo processInfo)
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

            processInfo.RemoveOnFinishHandler(this);

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

            _onFinishHandlers.Clear();
            _onCompleteHandlers.Clear();
            _onWeakCanceledHandlers.Clear();
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
            sb.AppendLine($"{spaces}{nameof(IsFinished)} = {IsFinished(null)}");
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
            sb.AppendLine($"{spaces}{nameof(IsFinished)} = {IsFinished(null)}");
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
            sb.AppendLine($"{spaces}{nameof(IsFinished)} = {IsFinished(null)}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);
            sb.PrintPODList(n, nameof(Friends), Friends);

            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");

            sb.PrintExisting(n, nameof(ParentProcessInfo), ParentProcessInfo);
            sb.PrintExisting(n, "Children", GetChildrenProcessInfoList);

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
        public List<MonitoredHumanizedLabel> ToChainOfProcessInfoLabels(IMonitorLogger logger)
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
            throw new NotImplementedException("6D1CBC4C-FA9A-43EB-A843-C5A5A4C1093E");
        }
        */
    }
}
