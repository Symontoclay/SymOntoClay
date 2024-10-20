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

using Newtonsoft.Json.Linq;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeExecution.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ExecutionCoordinator: IExecutionCoordinator
    {
        public ExecutionCoordinator(IInstance instance)
        {
            _instance = instance;
            Id = NameHelper.GetNewEntityNameString();
        }

        private readonly IInstance _instance;
        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public string Id { get; private set; }

        /// <inheritdoc/>
        public IInstance Instance => _instance;

        /// <inheritdoc/>
        public KindOfInstance KindOfInstance => _instance.KindOfInstance;

        private bool _isFinished = false;

        /// <inheritdoc/>
        public ActionExecutionStatus ExecutionStatus 
        {
            get 
            { 
                lock(_lockObj)
                {
                    return _executionStatus;
                }
            }
        }

        /// <inheritdoc/>
        public void SetExecutionStatus(IMonitorLogger logger, string messagePointId, ActionExecutionStatus actionExecutionStatus)
        {
            lock (_lockObj)
            {
                if (_executionStatus == actionExecutionStatus)
                {
                    return;
                }

                var prevExecutionStatus = _executionStatus;

                logger.SetExecutionCoordinatorStatus(messagePointId, Id, actionExecutionStatus, prevExecutionStatus, null, string.Empty);

                _executionStatus = actionExecutionStatus;

                var currIsFinished = _executionStatus != ActionExecutionStatus.Executing;

                if (currIsFinished != _isFinished)
                {
                    _isFinished = currIsFinished;

                    if (currIsFinished)
                    {
                        lock(_processInfosLockObj)
                        {
#if DEBUG
                            //logger.Info("A93BAEB1-27A0-4BE2-842F-38660C091ED5", $"actionExecutionStatus = {actionExecutionStatus}; prevExecutionStatus = {prevExecutionStatus}");
                            //logger.Info("E7B51F9E-DC79-422D-90BE-4EAE0D43B9CD", $"_processInfosList.Count = {_processInfosList.Count}");
#endif

                            if(_processInfosList.Any())
                            {
                                var processInfoStatus = ActionExecutionStatusHelper.ToProcessStatus(actionExecutionStatus);

#if DEBUG
                                //logger.Info("EA00316D-C87D-4AFB-8ECA-E6D38405CECC", $"processInfoStatus = {processInfoStatus}");
#endif

                                foreach (var processInfo in _processInfosList)
                                {
#if DEBUG
                                    //logger.Info("A4B5CAF6-F550-415D-B76D-95EC6F7471A4", $"processInfo = {processInfo.ToHumanizedLabel()}");
#endif

                                    processInfo.SetStatus(logger, messagePointId, processInfoStatus);
                                }
                            }
                        }

                        InternalOnFinish?.Invoke();
                    }
                }
            }
        }

        private ActionExecutionStatus _executionStatus;

        /// <inheritdoc/>
        public bool IsFinished 
        { 
            get
            {
                lock(_lockObj)
                {
                    return _isFinished;
                }
            }
        }

        /// <inheritdoc/>
        public RuleInstance RuleInstance
        {
            get
            {
                lock (_lockObj)
                {
                    return _ruleInstance;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    _ruleInstance = value;
                }
            }
        }

        private RuleInstance _ruleInstance;

        /// <inheritdoc/>
        public event Action OnFinished
        {
            add
            {
                InternalOnFinish += value;

                if (_isFinished)
                {
                    InternalOnFinish?.Invoke();
                }
            }

            remove
            {
                InternalOnFinish -= value;
            }
        }

        private event Action InternalOnFinish;

        /// <inheritdoc/>
        public void AddProcessInfo(IProcessInfo processInfo)
        {
            lock(_processInfosLockObj)
            {
                if(_processInfosList.Contains(processInfo))
                {
                    return;
                }

                _processInfosList.Add(processInfo);
            }
        }

        private readonly object _processInfosLockObj = new object();
        private List<IProcessInfo> _processInfosList = new List<IProcessInfo>();

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(ExecutionStatus)} = {ExecutionStatus}");
            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(ExecutionStatus)} = {ExecutionStatus}");
            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(ExecutionStatus)} = {ExecutionStatus}");
            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);

            return sb.ToString();
        }
    }
}
