/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
