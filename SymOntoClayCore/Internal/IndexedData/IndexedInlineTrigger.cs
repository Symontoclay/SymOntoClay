/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedInlineTrigger: IndexedAnnotatedItem
    {
        public InlineTrigger OriginalInlineTrigger { get; set; }
        public KindOfInlineTrigger Kind { get; set; } = KindOfInlineTrigger.Unknown;
        public KindOfSystemEventOfInlineTrigger KindOfSystemEvent { get; set; } = KindOfSystemEventOfInlineTrigger.Unknown;
        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ (ulong)Math.Abs(Kind.GetHashCode()) ^ (ulong)Math.Abs(KindOfSystemEvent.GetHashCode()) ^ CompiledFunctionBody.GetLongHashCode();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");
            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");
            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");
            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
