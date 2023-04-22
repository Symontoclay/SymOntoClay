using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ProcessInfoValue : Value
    {
        public ProcessInfoValue(IProcessInfo processInfo)
        {
            ProcessInfo = processInfo;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ProcessInfoValue;

        /// <inheritdoc/>
        public override bool IsProcessInfoValue => true;

        /// <inheritdoc/>
        public override ProcessInfoValue AsProcessInfoValue => this;

        public IProcessInfo ProcessInfo { get; private set; }

        public void AddOnFinishHandler(IProcessInfoEventHandler handler)
        {
            ProcessInfo.AddOnFinishHandler(handler);
        }

        public void RemoveOnFinishHandler(IProcessInfoEventHandler handler)
        {
            ProcessInfo.RemoveOnFinishHandler(handler);
        }

        public void AddOnCompleteHandler(IProcessInfoEventHandler handler)
        {
            ProcessInfo.AddOnCompleteHandler(handler);
        }

        public void RemoveOnCompleteHandler(IProcessInfoEventHandler handler)
        {
            ProcessInfo.RemoveOnCompleteHandler(handler);
        }

        public void AddOnWeakCanceledHandler(IProcessInfoEventHandler handler)
        {
            ProcessInfo.AddOnWeakCanceledHandler(handler);
        }

        public void RemoveOnWeakCanceledHandler(IProcessInfoEventHandler handler)
        {
            ProcessInfo.RemoveOnWeakCanceledHandler(handler);
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(ProcessInfo?.GetHashCode() ?? 0);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new ProcessInfoValue(ProcessInfo);
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(ProcessInfo), ProcessInfo);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(ProcessInfo), ProcessInfo);

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
            return $"proc: {ProcessInfo.Id} ({ProcessInfo.Status})";
        }
    }
}
