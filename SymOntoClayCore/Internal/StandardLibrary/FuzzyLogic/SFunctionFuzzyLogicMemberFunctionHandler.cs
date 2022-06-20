/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public class SFunctionFuzzyLogicMemberFunctionHandler : BaseFuzzyLogicMemberFunctionHandler
    {
        public SFunctionFuzzyLogicMemberFunctionHandler(NumberValue a, NumberValue b)
            : this((double)a.GetSystemValue(), (double)b.GetSystemValue())
        {
        }

        public SFunctionFuzzyLogicMemberFunctionHandler(double a, double b)
            : this(a, (a + b) / 2, b)
        {
        }

        public SFunctionFuzzyLogicMemberFunctionHandler(NumberValue a, NumberValue m, NumberValue b)
            : this((double)a.GetSystemValue(), (double)m.GetSystemValue(), (double)b.GetSystemValue())
        {
        }

        public SFunctionFuzzyLogicMemberFunctionHandler(double a, double m, double b)
            : base(a, b)
        {
            _a = a;
            _m = m;
            _b = b;
        }

        private readonly double _a;
        private readonly double _m;
        private readonly double _b;

        /// <inheritdoc/>
        public override KindOfFuzzyLogicMemberFunction Kind => KindOfFuzzyLogicMemberFunction.SFunction;

        /// <inheritdoc/>
        public override double SystemCall(double x)
        {
            return SystemMemberFunctions.SFunction(x, _a, _m, _b);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_m)} = {_m.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b.ToString(CultureInfo.InvariantCulture)}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_m)} = {_m.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b.ToString(CultureInfo.InvariantCulture)}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_m)} = {_m.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b.ToString(CultureInfo.InvariantCulture)}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
