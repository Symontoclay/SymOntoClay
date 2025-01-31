using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Handlers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ThreadExecutorValue : Value
    {
        public ThreadExecutorValue(IThreadExecutor threadExecutor) 
        {
            ThreadExecutor = threadExecutor;
            _threadExecutorCancelHandler = new ThreadExecutorCancelHandler(threadExecutor);
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ThreadExecutorValue;

        /// <inheritdoc/>
        public override bool IsThreadExecutorValue => true;

        /// <inheritdoc/>
        public override ThreadExecutorValue AsThreadExecutorValue => this;

        public IThreadExecutor ThreadExecutor { get; private set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return ThreadExecutor;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("C5B1FA86-0F62-49DC-B7DD-6FE685889A70");
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

            if (method != null)
            {
                return method;
            }

            return base.GetMethod(logger, methodName, kindOfParameters, namedParameters, positionedParameters);
        }

        private readonly ThreadExecutorCancelHandler _threadExecutorCancelHandler;

        private IExecutable GetCancelMethod(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters)
        {
            if (kindOfParameters != KindOfFunctionParameters.NoParameters)
            {
                return null;
            }

            return _threadExecutorCancelHandler;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(ThreadExecutor?.GetHashCode() ?? 0);
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

            var result = new ThreadExecutorValue(ThreadExecutor);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(ThreadExecutor)} = {ThreadExecutor?.GetHashCode()}");
            sb.AppendLine($"{spaces}ThreadExecutor?.RunningStatus = {ThreadExecutor?.RunningStatus}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(ThreadExecutor)} = {ThreadExecutor?.GetHashCode()}");
            sb.AppendLine($"{spaces}ThreadExecutor?.RunningStatus = {ThreadExecutor?.RunningStatus}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(ThreadExecutor)} = {ThreadExecutor?.GetHashCode()}");
            sb.AppendLine($"{spaces}ThreadExecutor?.RunningStatus = {ThreadExecutor?.RunningStatus}");

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
            return $"ThreadExecutor: {ThreadExecutor?.GetHashCode()} ({ThreadExecutor?.RunningStatus})";
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
