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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core
{
    public abstract class BaseProcessInfo : IProcessInfo
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        protected BaseProcessInfo()
        {
            Id = NameHelper.GetNewEntityNameString();
        }

        /// <inheritdoc/>
        public string Id { get; private set; }

        /// <inheritdoc/>
        public abstract string EndPointName { get; }

        /// <inheritdoc/>
        public abstract ProcessStatus Status { get; set; }

        /// <inheritdoc/>
        public bool IsFinished
        {
            get
            {
                lock(_statusLockObj)
                {
                    return NIsFinished;
                }
            }
        }

        protected bool NIsFinished
        {
            get
            {
                var status = Status;
                return status == ProcessStatus.Completed || status == ProcessStatus.Canceled || status == ProcessStatus.WeakCanceled || status == ProcessStatus.Faulted;
            }
        }

        /// <inheritdoc/>
        public abstract IReadOnlyList<int> Devices { get; }

        /// <inheritdoc/>
        public abstract event ProcessInfoEvent OnFinish;

        /// <inheritdoc/>
        public abstract event ProcessInfoEvent OnComplete;

        /// <inheritdoc/>
        public abstract void Start();

        /// <inheritdoc/>
        public abstract void Cancel();

        /// <inheritdoc/>
        public abstract void WeakCancel();

        protected void NCancelChildren()
        {
#if DEBUG
            //_gbcLogger.Info($"GetHashCode() = {GetHashCode()}");
#endif

            foreach (var child in _childrenProcessInfoList.ToList())
            {
#if DEBUG
                //_gbcLogger.Info($"child.GetHashCode() = {child.GetHashCode()}");
#endif

                child.Cancel();
            }
        }

        protected void NWeakCancelChildren()
        {
#if DEBUG
            //_gbcLogger.Info($"GetHashCode() = {GetHashCode()}");
#endif

            foreach (var child in _childrenProcessInfoList.ToList())
            {
#if DEBUG
                //_gbcLogger.Info($"child.GetHashCode() = {child.GetHashCode()}");
#endif

                child.WeakCancel();
            }
        }

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
                if(processInfo == this)
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

        #region private fields
        protected readonly object _statusLockObj = new object();
        protected readonly object _parentAndChildrenLockObj = new object();
        private IProcessInfo _parentProcessInfo;
        private readonly List<IProcessInfo> _childrenProcessInfoList = new List<IProcessInfo>();
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
