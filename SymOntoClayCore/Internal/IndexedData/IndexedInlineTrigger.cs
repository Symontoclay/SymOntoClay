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
