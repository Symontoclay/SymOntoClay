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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FuzzyLogicNonNumericSequenceValue : Value
    {
        public void AddIdentifier(StrongIdentifierValue identifier)
        {
            if(NonNumericValue == null)
            {
                NonNumericValue = identifier;
            }
            else
            {
                _operators.Add(NonNumericValue);
                NonNumericValue = identifier;
            }
        }

        public StrongIdentifierValue NonNumericValue { get; private set; }
        public IEnumerable<StrongIdentifierValue> Operators => _operators;

        private List<StrongIdentifierValue> _operators = new List<StrongIdentifierValue>();

        public bool Equals(FuzzyLogicNonNumericSequenceValue anotherSequence)
        {
            if(NonNumericValue != anotherSequence.NonNumericValue)
            {
                return false;
            }

            var opEnumerator = anotherSequence.Operators.GetEnumerator();

            foreach(var op in _operators)
            {
                if(!opEnumerator.MoveNext())
                {
                    return false;
                }

                if(op != opEnumerator.Current)
                {
                    return false;
                }
            }

            return true;
        }

        private string _debugView = string.Empty;

        public string DebugView => _debugView;

        private string _systemValue = string.Empty;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.FuzzyLogicNonNumericSequenceValue;

        /// <inheritdoc/>
        public override bool IsFuzzyLogicNonNumericSequenceValue => true;

        /// <inheritdoc/>
        public override FuzzyLogicNonNumericSequenceValue AsFuzzyLogicNonNumericSequenceValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return _systemValue;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            return _systemValue;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _systemValue.GetHashCode();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            var systemValuesList = new List<string>();
            var debugViewsList = new List<string>();

            if (!_operators.IsNullOrEmpty())
            {
                foreach(var op in _operators)
                {
                    result ^= op.GetLongHashCode(options);
                    systemValuesList.Add(op.NormalizedNameValue);
                    debugViewsList.Add(op.NameValue);
                }
            }

            if(NonNumericValue != null)
            {
                result ^= NonNumericValue.GetLongHashCode(options);
                systemValuesList.Add(NonNumericValue.NormalizedNameValue);
                debugViewsList.Add(NonNumericValue.NameValue);
            }

            _systemValue = string.Join(" ", systemValuesList);
            _debugView = string.Join(" ", debugViewsList);

            return result;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public FuzzyLogicNonNumericSequenceValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public FuzzyLogicNonNumericSequenceValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (FuzzyLogicNonNumericSequenceValue)context[this];
            }

            var result = new FuzzyLogicNonNumericSequenceValue();
            context[this] = result;

            result.NonNumericValue = NonNumericValue?.Clone(context);
            result._operators = _operators?.Select(p => p.Clone(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(NonNumericValue), NonNumericValue);
            sb.PrintObjListProp(n, nameof(Operators), Operators);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(NonNumericValue), NonNumericValue);
            sb.PrintShortObjListProp(n, nameof(Operators), Operators);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(NonNumericValue), NonNumericValue);
            sb.PrintBriefObjListProp(n, nameof(Operators), Operators);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}`{_debugView}`";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            return $"`{_debugView}`";
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
