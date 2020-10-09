/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.IndexedData
{
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
