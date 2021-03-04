/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.IndexedData
{
    [Obsolete("IndexedData must be removed!", true)]
    public class IndexedTaskValue : IndexedValue
    {
        public TaskValue OriginalTaskValue { get; set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.TaskValue;

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalTaskValue;

        public string TaskId { get; set; }
        public ulong TaskIdKey { get; set; }
        public Task SystemTask { get; set; }

        /// <inheritdoc/>
        public override bool IsTaskValue => true;

        /// <inheritdoc/>
        public override IndexedTaskValue AsTaskValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return SystemTask;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ TaskIdKey ^ (ulong)Math.Abs(SystemTask?.GetHashCode() ?? 0);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}{nameof(TaskIdKey)} = {TaskIdKey}");
            sb.AppendLine($"{spaces}{SystemTask?.Status} = {SystemTask?.Status}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}{nameof(TaskIdKey)} = {TaskIdKey}");
            sb.AppendLine($"{spaces}{SystemTask?.Status} = {SystemTask?.Status}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}{nameof(TaskIdKey)} = {TaskIdKey}");
            sb.AppendLine($"{spaces}{SystemTask?.Status} = {SystemTask?.Status}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
