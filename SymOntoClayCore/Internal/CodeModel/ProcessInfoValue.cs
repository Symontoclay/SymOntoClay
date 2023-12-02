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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
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

        public void AddOnFinishHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
        {
            ProcessInfo.AddOnFinishHandler(logger, handler);
        }

        public void RemoveOnFinishHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
        {
            ProcessInfo.RemoveOnFinishHandler(logger, handler);
        }

        public void AddOnCompleteHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
        {
            ProcessInfo.AddOnCompleteHandler(logger, handler);
        }

        public void RemoveOnCompleteHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
        {
            ProcessInfo.RemoveOnCompleteHandler(logger, handler);
        }

        public void AddOnWeakCanceledHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
        {
            ProcessInfo.AddOnWeakCanceledHandler(logger, handler);
        }

        public void RemoveOnWeakCanceledHandler(IMonitorLogger logger, IProcessInfoEventHandler handler)
        {
            ProcessInfo.RemoveOnWeakCanceledHandler(logger, handler);
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
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new ProcessInfoValue(ProcessInfo);
            context[this] = result;

            result.AppendAnnotations(this, context);

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
            return ProcessInfo.ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}
