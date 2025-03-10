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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class StringValue: Value
    {
        private static List<TypeInfo> _builtInSuperTypes;

        static StringValue()
        {
            _builtInSuperTypes = new List<TypeInfo>();
            _builtInSuperTypes.Add(NameHelper.CreateName(StandardNamesConstants.StringTypeName));
        }

        public StringValue(string systemValue)
        {
            SystemValue = systemValue;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.StringValue;

        /// <inheritdoc/>
        public override bool IsStringValue => true;

        /// <inheritdoc/>
        public override StringValue AsStringValue => this;

        /// <inheritdoc/>
        public override IReadOnlyList<TypeInfo> BuiltInSuperTypes => _builtInSuperTypes;

        public string SystemValue { get; private set; }

        /// <inheritdoc/>
        public override bool IsSystemNull => SystemValue == null;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return SystemValue;
        }

        public RuleInstance ToRuleInstance(IMonitorLogger logger, IEngineContext engineContext)
        {
            lock(_toRuleInstanceValueLockObj)
            {
                if (_usedSystemValueForRuleInstanceValue == SystemValue)
                {
                    return _ruleInstance;
                }

                _usedSystemValueForRuleInstanceValue = SystemValue;

                var converter = engineContext.NLPConverterFactory?.GetConverter();

                if(converter == null)
                {
                    throw new ArgumentNullException(nameof(converter));
                }

                var factsList = converter.Convert(logger, _usedSystemValueForRuleInstanceValue);

                if(factsList.IsNullOrEmpty())
                {
                    return null;
                }

                if(factsList.Count > 1)
                {
                    throw new NotImplementedException("E670C1CB-8D18-4471-BB77-1988307386B7");
                }

                _ruleInstance = factsList.Single();

                _ruleInstance.CheckDirty();

                return _ruleInstance;
            }
        }

        private readonly object _toRuleInstanceValueLockObj = new object();
        private RuleInstance _ruleInstance;
        private string _usedSystemValueForRuleInstanceValue;

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            return SystemValue;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(SystemValue?.GetHashCode() ?? 0);
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

            var result = new StringValue(SystemValue);
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
            return $"{spaces}'{SystemValue}'";
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

            return $"{SystemValue}";
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
