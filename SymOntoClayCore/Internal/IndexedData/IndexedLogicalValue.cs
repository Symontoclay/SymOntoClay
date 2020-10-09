/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedLogicalValue: IndexedValue
    {
        public LogicalValue OriginalLogicalValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalLogicalValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.LogicalValue;

        /// <inheritdoc/>
        public override bool IsLogicalValue => true;

        /// <inheritdoc/>
        public override IndexedLogicalValue AsLogicalValue => this;

        public float? SystemValue { get; set; }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return SystemValue;
        }

        public IndexedLogicalValue Inverse(IMainStorageContext mainStorageContext)
        {
            var result = OriginalLogicalValue.Inverse();

            return result.GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ (ulong)Math.Abs(SystemValue?.GetHashCode() ?? 0);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{SystemValue}";
        }
    }
}
