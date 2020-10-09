/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
