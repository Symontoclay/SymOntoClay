using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class MutablePartOfRuleInstanceValue : Value
    {
        public MutablePartOfRuleInstanceValue(MutablePartOfRuleInstance mutablePartOfRuleInstance)
        {
            MutablePartOfRuleInstance = mutablePartOfRuleInstance;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.MutablePartOfRuleInstance;

        /// <inheritdoc/>
        public override bool IsMutablePartOfRuleInstanceValue => true;

        /// <inheritdoc/>
        public override MutablePartOfRuleInstanceValue AsMutablePartOfRuleInstanceValue => this;

        public MutablePartOfRuleInstance MutablePartOfRuleInstance { get; private set; }

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return MutablePartOfRuleInstance;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.FactTypeName) };

            return base.CalculateLongHashCode(options);
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

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public MutablePartOfRuleInstanceValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public MutablePartOfRuleInstanceValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (MutablePartOfRuleInstanceValue)context[this];
            }

            var result = new MutablePartOfRuleInstanceValue(MutablePartOfRuleInstance.Clone(context));
            context[this] = result;

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(MutablePartOfRuleInstance), MutablePartOfRuleInstance);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(MutablePartOfRuleInstance), MutablePartOfRuleInstance);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(MutablePartOfRuleInstance), MutablePartOfRuleInstance);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{DebugHelperForRuleInstance.ToString(MutablePartOfRuleInstance.Parent, MutablePartOfRuleInstance)}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return DebugHelperForRuleInstance.ToString(MutablePartOfRuleInstance.Parent, MutablePartOfRuleInstance, options);
        }
    }
}
