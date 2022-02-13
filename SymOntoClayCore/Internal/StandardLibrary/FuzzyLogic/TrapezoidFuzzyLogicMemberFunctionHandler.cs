/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public class TrapezoidFuzzyLogicMemberFunctionHandler : BaseFuzzyLogicMemberFunctionHandler
    {
        public TrapezoidFuzzyLogicMemberFunctionHandler(NumberValue a, NumberValue b, NumberValue c, NumberValue d)
            :this((double)a.GetSystemValue(), (double)b.GetSystemValue(), (double) c.GetSystemValue(), (double) d.GetSystemValue())
        {
        }

        public TrapezoidFuzzyLogicMemberFunctionHandler(double a, double b, double c, double d)
            : base(a, d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }

        private readonly double _a;
        private readonly double _b;
        private readonly double _c;
        private readonly double _d;

        /// <inheritdoc/>
        public override KindOfFuzzyLogicMemberFunction Kind => KindOfFuzzyLogicMemberFunction.Trapezoid;

        /// <inheritdoc/>
        public override double SystemCall(double x)
        {
            return SystemMemberFunctions.Trapezoid(x, _a, _b, _c, _d);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_c)} = {_c.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_d)} = {_d.ToString(CultureInfo.InvariantCulture)}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_c)} = {_c.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_d)} = {_d.ToString(CultureInfo.InvariantCulture)}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_c)} = {_c.ToString(CultureInfo.InvariantCulture)}");
            sb.AppendLine($"{spaces}{nameof(_d)} = {_d.ToString(CultureInfo.InvariantCulture)}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
