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

using NLog;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Handlers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class TaskValue : Value
    {
        public TaskValue(IThreadTask systemTask)
        {
            SystemTask = systemTask;
            TaskId = Guid.NewGuid().ToString("D");
            _cancellationTokenSourceCancelHandler = new CancellationTokenSourceCancelHandler(systemTask.CancellationTokenSource);
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.TaskValue;

        /// <inheritdoc/>
        public override bool IsTaskValue => true;

        /// <inheritdoc/>
        public override TaskValue AsTaskValue => this;

        public string TaskId { get; set; }
        public IThreadTask SystemTask 
        { 
            get
            {
                return _systemTask;
            }

            set
            {
                if (_systemTask == value)
                {
                    return;
                }

                if(_systemTask != null)
                {
                    _systemTask.OnCompletedSuccessfully -= OnCompleteHandler;
                }

                _systemTask = value;

                if (_systemTask != null)
                {
                    _systemTask.OnCompletedSuccessfully += OnCompleteHandler;
                }
            }
        }

        private IThreadTask _systemTask;

        public event Action OnComplete
        {
            add
            {
                InternalOnComplete += value;

                if(_systemTask != null)
                {
                    if(_systemTask.Status == ThreadTaskStatus.RanToCompletion)
                    {
                        value?.Invoke();
                    }
                }
            }

            remove 
            { 
                InternalOnComplete -= value; 
            }
        }

        private event Action InternalOnComplete;

        private object _checkOnCompleteLockObj = new object();

        private void OnCompleteHandler()
        {
            InternalOnComplete?.Invoke();
        }

        public void Wait()
        {
            _systemTask?.Wait();
        }

        public void Cancel()
        {
            _systemTask?.Cancel();
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return _systemTask;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("5B573736-20D4-4A82-8E40-15739AC8B561");
        }

        /// <inheritdoc/>
        public override IExecutable GetMethod(IMonitorLogger logger, StrongIdentifierValue methodName, KindOfFunctionParameters kindOfParameters, Dictionary<StrongIdentifierValue, Value> namedParameters, List<Value> positionedParameters)
        {
            var methodNameStr = methodName.NormalizedNameValue;

            IExecutable method = null;

            switch (methodNameStr)
            {
                case "cancel":
                    method = GetCancelMethod(logger, kindOfParameters);
                    break;

            }

            if(method != null)
            {
                return method;
            }

            return base.GetMethod(logger, methodName, kindOfParameters, namedParameters, positionedParameters);
        }

        private readonly CancellationTokenSourceCancelHandler _cancellationTokenSourceCancelHandler;

        private IExecutable GetCancelMethod(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters)
        {
            if(kindOfParameters != KindOfFunctionParameters.NoParameters)
            {
                return null;
            }

            return _cancellationTokenSourceCancelHandler;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)TaskId.GetHashCode() ^ (ulong)Math.Abs(_systemTask?.GetHashCode() ?? 0);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new TaskValue(_systemTask);
            context[this] = result;

            result.TaskId = TaskId;
            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}SystemTask?.Status = {_systemTask?.Status}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}SystemTask?.Status = {_systemTask?.Status}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}SystemTask?.Status = {_systemTask?.Status}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            return $"Task: {TaskId} ({SystemTask?.Status})";
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }
    }
}
