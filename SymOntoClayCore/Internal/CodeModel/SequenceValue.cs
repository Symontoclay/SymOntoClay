using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SequenceValue : Value
    {
        public SequenceValue()
        {
        }

        public SequenceValue(Value initialValue)
        {
            _values.Add(initialValue);
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.SequenceValue;

        /// <inheritdoc/>
        public override bool IsSequenceValue => true;

        /// <inheritdoc/>
        public override SequenceValue AsSequenceValue => this;

        private List<Value> _values = new List<Value>();

        public List<Value> Values => _values;

        public void AddValue(Value value)
        {
            _values.Add(value);
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return ToSystemString();
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            return string.Join(" ", _values.Select(p => p.ToSystemString()));
        }

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.SequenceTypeName) };

            var result = base.CalculateLongHashCode(options);

            foreach(var value in _values)
            {
                result ^= value.GetLongHashCode(options);
            }

            return result;
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

            var result = new SequenceValue();
            cloneContext[this] = result;

            result._values = _values?.Select(p => p.CloneValue(cloneContext)).ToList();

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjListProp(n, nameof(Values), Values);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintShortObjListProp(n, nameof(Values), Values);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(Values), Values);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return string.Join(" ", _values.Select(p => p.ToHumanizedString(options)));
        }
    }
}
