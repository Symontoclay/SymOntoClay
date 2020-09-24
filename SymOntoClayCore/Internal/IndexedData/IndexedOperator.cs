using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedOperator : IndexedAnnotatedItem, IExecutable
    {
        public Operator OriginalOperator { get; set; }

        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;

        /// <inheritdoc/>
        public bool IsSystemDefined { get; set; }

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        public ISystemHandler SystemHandler { get; set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode());

            if(IsSystemDefined)
            {
                result ^= (ulong)Math.Abs(SystemHandler.GetHashCode());
            }
            else
            {
                result ^= CompiledFunctionBody.GetLongHashCode();
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
