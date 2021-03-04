/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    [Obsolete("IndexedData must be removed!", true)]
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
        public override ulong GetLongConditionalHashCode()
        {
            return IndexedRuleInstance.GetLongConditionalHashCode();
        }

        /// <inheritdoc/>
        public override ulong GetLongHashCode()
        {
            return IndexedRuleInstance.GetLongHashCode();
        }

        /// <inheritdoc/>
        protected override void CalculateLongConditionalHashCode()
        {
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return IndexedRuleInstance.GetLongHashCode();
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
