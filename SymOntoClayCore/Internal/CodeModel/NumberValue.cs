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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class NumberValue : Value
    {
        public NumberValue(double? systemValue)
        {
            SystemValue = systemValue;
        }
        
        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.NumberValue;

        /// <inheritdoc/>
        public override bool IsNumberValue => true;

        /// <inheritdoc/>
        public override NumberValue AsNumberValue => this;

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;
        
        private bool _isFuzzy;
        private bool _isBoolean;

        public bool IsFuzzy => _isFuzzy;
        public bool IsBoolean => _isBoolean;

        public double? SystemValue { get; private set; }

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
            return SystemValue == other.AsNumberValue.SystemValue;
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
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            if(SystemValue.HasValue)
            {
                if(SystemValue >= 0 && SystemValue <= 1)
                {
                    _isFuzzy = true;

                    if(SystemValue == 0 || SystemValue == 1)
                    {
                        _isBoolean = true;
                    }
                }
            }
            else
            {
                _isFuzzy = true;
                _isBoolean = true;
            }

            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.NumberTypeName) };

            if(_isFuzzy)
            {
                _builtInSuperTypes.Add(NameHelper.CreateName(StandardNamesConstants.FuzzyTypeName));
            }

            if(_isBoolean)
            {
                _builtInSuperTypes.Add(NameHelper.CreateName(StandardNamesConstants.BooleanTypeName));
            }

            return base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(SystemValue?.GetHashCode() ?? 0);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public NumberValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public NumberValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (NumberValue)context[this];
            }

            var result = new NumberValue(SystemValue);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
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

        /// <inheritdoc/>
        public override object ToMonitorSerializableObject(IMonitorLogger logger)
        {
            return SystemValue;
        }
    }
}
