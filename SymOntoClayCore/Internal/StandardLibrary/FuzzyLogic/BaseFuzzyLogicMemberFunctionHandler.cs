/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public abstract class BaseFuzzyLogicMemberFunctionHandler: IFuzzyLogicMemberFunctionHandler
    {
        private static IMonitorLogger _logger = MonitorLoggerNLogImpementation.Instance;

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
        public double SystemCall(IMonitorLogger logger, NumberValue x)
        {
            return SystemCall(logger, (double)x.GetSystemValue());
        }

        /// <inheritdoc/>
        public abstract double SystemCall(IMonitorLogger logger, double x);

        private ulong? _longHashCode;

        /// <inheritdoc/>
        public ulong GetLongHashCode()
        {
            if (!_longHashCode.HasValue)
            {
                Check(_logger);
                _isDirty = false;
            }

            return _longHashCode.Value;
        }

        /// <inheritdoc/>
        public NumberValue Defuzzificate(IMonitorLogger logger)
        {
            return _deffuzificatedValue;
        }

        /// <inheritdoc/>
        public NumberValue Defuzzificate(IMonitorLogger logger, IEnumerable<IFuzzyLogicOperatorHandler> operatorHandlers)
        {
            var sysDeffuzificatedValue = Defuzzificator.Defuzzificate(_kindOfDefuzzification, _a, _b, (double x) => {
                var result = SystemCall(logger, x);

                foreach(var op in operatorHandlers)
                {
                    result = op.SystemCall(logger, result);
                }

                return result;
            });

            return new NumberValue(sysDeffuzificatedValue);
        }

        private bool _isDirty = true;

        public void CheckDirty(IMonitorLogger logger)
        {
            if (_isDirty)
            {
                Check(logger);
                _isDirty = false;
            }
        }

        /// <inheritdoc/>
        public virtual void Check(IMonitorLogger logger)
        {
            _longHashCode = LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(Kind.GetHashCode());

            var sysDeffuzificatedValue = Defuzzificator.Defuzzificate(_kindOfDefuzzification, _a, _b, (double x) => SystemCall(logger,x));

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
