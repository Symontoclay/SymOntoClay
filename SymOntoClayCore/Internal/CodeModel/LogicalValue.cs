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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalValue: Value
    {
        public static readonly LogicalValue NullValue = new LogicalValue(null);
        public static readonly LogicalValue TrueValue = new LogicalValue(true);
        public static readonly LogicalValue FalseValue = new LogicalValue(false);

        static LogicalValue()
        {
            NullValue.CheckDirty();
            TrueValue.CheckDirty();
            FalseValue.CheckDirty();
        }

        public LogicalValue(bool systemBool)
            : this(systemBool ? 1: 0)
        {
        }

        public LogicalValue(float? systemValue)
        {
            if(systemValue.HasValue)
            {
                if (systemValue > 1F || systemValue < 0F)
                {
                    throw new ArgumentOutOfRangeException(nameof(systemValue), systemValue, "The system (C#) value which represents SymOntoClay's logical value must be between 0 and 1.");
                }
            }

            SystemValue = systemValue;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.LogicalValue;

        /// <inheritdoc/>
        public override bool IsLogicalValue => true;

        /// <inheritdoc/>
        public override LogicalValue AsLogicalValue => this;

        public float? SystemValue { get; private set; }

        public LogicalValue Inverse()
        {
            if(SystemValue.HasValue)
            {
                return new LogicalValue(SystemFuzzyLogicOperators.Not(SystemValue.Value));
            }

            return new LogicalValue(null);
        }

        public static LogicalValue Or(LogicalValue left, LogicalValue right)
        {
            if(!left.SystemValue.HasValue || !right.SystemValue.HasValue)
            {
                return NullValue;
            }

            return new LogicalValue(SystemFuzzyLogicOperators.Or(left.SystemValue.Value, right.SystemValue.Value));
        }

        public static LogicalValue And(LogicalValue left, LogicalValue right)
        {
            if (!left.SystemValue.HasValue || !right.SystemValue.HasValue)
            {
                return NullValue;
            }

            return new LogicalValue(SystemFuzzyLogicOperators.And(left.SystemValue.Value, right.SystemValue.Value));
        }

        /// <inheritdoc/>
        public override bool IsSystemNull => !SystemValue.HasValue;

        /// <inheritdoc/>
        public override bool NullValueEquals()
        {
            return !SystemValue.HasValue;
        }

        /// <inheritdoc/>
        protected override bool ConcreteValueEquals(Value other)
        {
            return SystemValue == other.AsLogicalValue.SystemValue;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return SystemValue;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            return SystemValue.ToString();
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new LogicalValue(SystemValue);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(SystemValue?.GetHashCode() ?? 0);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{SystemValue}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            if (SystemValue == null)
            {
                return "NULL";
            }

            return SystemValue.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }
    }
}
