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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class StringValue: Value
    {
        private static List<StrongIdentifierValue> _builtInSuperTypes;

        static StringValue()
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>();
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
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        public string SystemValue { get; private set; }

        /// <inheritdoc/>
        public override bool IsSystemNull => SystemValue == null;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return SystemValue;
        }

        public RuleInstanceValue ToRuleInstanceValue(IEngineContext engineContext)
        {
            lock(_toRuleInstanceValueLockObj)
            {
                if (_usedSystemValueForRuleInstanceValue == SystemValue)
                {
                    return _ruleInstanceValue;
                }

                _usedSystemValueForRuleInstanceValue = SystemValue;

                var converter = engineContext.NLPConverterFactory?.GetConverter();

                if(converter == null)
                {
                    throw new ArgumentNullException(nameof(converter));
                }

                var factsList = converter.Convert(_usedSystemValueForRuleInstanceValue);

                if(factsList.IsNullOrEmpty())
                {
                    return null;
                }

                if(factsList.Count > 1)
                {
                    throw new NotImplementedException();
                }

                _ruleInstanceValue = new RuleInstanceValue(factsList.Single());

                _ruleInstanceValue.CheckDirty();

                return _ruleInstanceValue;
            }
        }

        private readonly object _toRuleInstanceValueLockObj = new object();
        private RuleInstanceValue _ruleInstanceValue;
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
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new StringValue(SystemValue);
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

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
        public override string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return NToHumanizedString();
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
    }
}
