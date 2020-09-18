using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedRuleInstanceValue : IndexedValue
    {
        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.RuleInstanceValue;

        /// <inheritdoc/>
        public override bool IsRuleInstanceValue => true;

        /// <inheritdoc/>
        public override IndexedRuleInstanceValue AsRuleInstanceValue => this;

        public RuleInstanceValue OriginalRuleInstanceValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalRuleInstanceValue;

        public RuleInstance RuleInstance { get; set; }
        public IndexedRuleInstance IndexedRuleInstance { get; set; }

        /// <inheritdoc/>
        [ResolveToType(typeof(LogicalValue))]
        public override IList<IndexedValue> QuantityQualityModalities { get => IndexedRuleInstance.QuantityQualityModalities; set => IndexedRuleInstance.QuantityQualityModalities = value; }

        /// <inheritdoc/>
        [ResolveToType(typeof(LogicalValue))]
        public override IList<IndexedValue> WhereSection { get => IndexedRuleInstance.WhereSection; set => IndexedRuleInstance.WhereSection = value; }

        /// <inheritdoc/>
        public override IndexedStrongIdentifierValue Holder { get => IndexedRuleInstance.Holder; set => IndexedRuleInstance.Holder = value; }

        /// <inheritdoc/>
        public override IList<IndexedLogicalAnnotation> Annotations { get => IndexedRuleInstance.Annotations; set => IndexedRuleInstance.Annotations = value; }

        /// <inheritdoc/>
        public override long GetLongConditionalHashCode()
        {
            return IndexedRuleInstance.GetLongConditionalHashCode();
        }

        /// <inheritdoc/>
        public override void CalculateLongConditionalHashCode()
        {
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return RuleInstance;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(IndexedRuleInstance), IndexedRuleInstance);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(IndexedRuleInstance), IndexedRuleInstance);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(IndexedRuleInstance), IndexedRuleInstance);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}ref: {IndexedRuleInstance.GetDefaultToDbgStringInformation(0u)}";
        }
    }
}
