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
