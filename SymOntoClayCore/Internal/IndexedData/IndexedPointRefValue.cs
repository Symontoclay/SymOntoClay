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

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedPointRefValue : IndexedValue
    {
        public PointRefValue OriginalPointRefValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalPointRefValue;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.PointRefValue;

        /// <inheritdoc/>
        public override bool IsPointRefValue => true;

        /// <inheritdoc/>
        public override IndexedPointRefValue AsPointRefValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return this;
        }

        public IndexedValue LeftOperand { get; set; }
        public IndexedValue RightOperand { get; set; }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode();

            if(LeftOperand != null)
            {
                result ^= LongHashCodeWeights.BaseParamWeight ^ LeftOperand.GetLongHashCode();
            }

            if (RightOperand != null)
            {
                result ^= LongHashCodeWeights.BaseParamWeight ^ RightOperand.GetLongHashCode();
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintObjProp(n, nameof(RightOperand), RightOperand);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintShortObjProp(n, nameof(RightOperand), RightOperand);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(LeftOperand), LeftOperand);
            sb.PrintBriefObjProp(n, nameof(RightOperand), RightOperand);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{LeftOperand.ToDbgString()}.{RightOperand.ToDbgString()}";
        }
    }
}
