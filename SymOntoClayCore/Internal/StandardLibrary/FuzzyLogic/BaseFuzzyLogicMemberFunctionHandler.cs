using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public abstract class BaseFuzzyLogicMemberFunctionHandler: IFuzzyLogicMemberFunctionHandler
    {
        protected BaseFuzzyLogicMemberFunctionHandler(double a, double b)
            : this(KindOfDefuzzification.CoG, a, b)
        {
        }

        protected BaseFuzzyLogicMemberFunctionHandler(KindOfDefuzzification kindOfDefuzzification, double a, double b)
        {
            _kindOfDefuzzification = kindOfDefuzzification;
            _a = a;
            _b = b;
        }

        private KindOfDefuzzification _kindOfDefuzzification;
        private readonly double _a;
        private readonly double _b;
        private NumberValue _deffuzificatedValue;

        /// <inheritdoc/>
        public abstract KindOfFuzzyLogicMemberFunction Kind { get; }

        /// <inheritdoc/>
        public double SystemCall(NumberValue x)
        {
            return SystemCall((double)x.GetSystemValue());
        }

        /// <inheritdoc/>
        public abstract double SystemCall(double x);

        private ulong? _longHashCode;

        /// <inheritdoc/>
        public ulong GetLongHashCode()
        {
            if (!_longHashCode.HasValue)
            {
                Check();
                _isDirty = false;
            }

            return _longHashCode.Value;
        }

        /// <inheritdoc/>
        public NumberValue Defuzzificate()
        {
            return _deffuzificatedValue;
        }

        /// <inheritdoc/>
        public NumberValue Defuzzificate(IEnumerable<IFuzzyLogicOperatorHandler> operatorHandlers)
        {
            var sysDeffuzificatedValue = Defuzzificator.Defuzzificate(_kindOfDefuzzification, _a, _b, (double x) => {
                var result = SystemCall(x);

                foreach(var op in operatorHandlers)
                {
                    result = op.SystemCall(result);
                }

                return result;
            });

            return new NumberValue(sysDeffuzificatedValue);
        }

        private bool _isDirty = true;

        public void CheckDirty()
        {
            if (_isDirty)
            {
                Check();
                _isDirty = false;
            }
        }

        /// <inheritdoc/>
        public virtual void Check()
        {
            _longHashCode = LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(Kind.GetHashCode());

            var sysDeffuzificatedValue = Defuzzificator.Defuzzificate(_kindOfDefuzzification, _a, _b, (double x) => SystemCall(x));

            _deffuzificatedValue = new NumberValue(sysDeffuzificatedValue);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }

        /// <inheritdoc/>
        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            return PropertiesToShortString(n);
        }

        /// <inheritdoc/>
        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        /// <inheritdoc/>
        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            return sb.ToString();
        }
    }
}
