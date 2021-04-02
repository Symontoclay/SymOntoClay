using NLog;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FuzzyLogicNonNumericSequenceValue : Value
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public void AddIdentifier(StrongIdentifierValue identifier)
        {
#if DEBUG
            _gbcLogger.Info($"identifier = {identifier}");
#endif

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
        public override int GetHashCode()
        {
            return _systemValue.GetHashCode();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode();

            var systemValuesList = new List<string>();
            var debugViewsList = new List<string>();

            if (!_operators.IsNullOrEmpty())
            {
                foreach(var op in _operators)
                {
                    result ^= op.GetLongHashCode();
                    systemValuesList.Add(op.NormalizedNameValue);
                    debugViewsList.Add(op.NameValue);
                }
            }

            if(NonNumericValue != null)
            {
                result ^= NonNumericValue.GetLongHashCode();
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

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public FuzzyLogicNonNumericSequenceValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
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
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
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
    }
}
