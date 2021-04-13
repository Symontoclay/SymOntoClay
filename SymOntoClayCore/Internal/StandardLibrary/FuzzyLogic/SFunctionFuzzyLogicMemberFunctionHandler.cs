using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
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

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a}");
            sb.AppendLine($"{spaces}{nameof(_m)} = {_m}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a}");
            sb.AppendLine($"{spaces}{nameof(_m)} = {_m}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a}");
            sb.AppendLine($"{spaces}{nameof(_m)} = {_m}");
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
