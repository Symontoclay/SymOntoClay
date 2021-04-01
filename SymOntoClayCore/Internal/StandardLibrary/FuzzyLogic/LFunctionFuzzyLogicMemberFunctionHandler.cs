using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public class LFunctionFuzzyLogicMemberFunctionHandler: BaseFuzzyLogicMemberFunctionHandler
    {
        public LFunctionFuzzyLogicMemberFunctionHandler(NumberValue a, NumberValue b)
            : this((double)a.GetSystemValue(), (double)b.GetSystemValue())
        {
        }

        public LFunctionFuzzyLogicMemberFunctionHandler(double a, double b)
        {
            _a = a;
            _b = b;
        }

        /// <inheritdoc/>
        public override KindOfFuzzyLogicMemberFunction Kind => KindOfFuzzyLogicMemberFunction.LFunction;

        private readonly double _a; 
        private readonly double _b;

        /// <inheritdoc/>
        public override double SystemCall(double x)
        {
            return SystemMemberFunctions.LFunction(x, _a, _b);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(_a)} = {_a}");
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
            sb.AppendLine($"{spaces}{nameof(_b)} = {_b}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
