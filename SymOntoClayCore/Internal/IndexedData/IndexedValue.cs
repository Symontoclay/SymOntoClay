using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public abstract class IndexedValue: IndexedAnnotatedItem
    {
        public abstract Value OriginalValue { get; }

        public abstract KindOfValue KindOfValue { get; }

        public virtual bool IsNullValue => false;
        public virtual IndexedNullValue AsNullValue => null;

        public virtual bool IsLogicalValue => false;
        public virtual IndexedLogicalValue AsLogicalValue => null;

        public virtual bool IsNumberValue => false;
        public virtual IndexedNumberValue AsNumberValue => null;

        public virtual bool IsStringValue => false;
        public virtual IndexedStringValue AsStringValue => null;

        public virtual bool IsStrongIdentifierValue => false;
        public virtual IndexedStrongIdentifierValue AsStrongIdentifierValue => null;

        public virtual bool IsTaskValue => false;
        public virtual IndexedTaskValue AsTaskValue => null;

        public virtual bool IsAnnotationValue => false;
        public virtual IndexedAnnotationValue AsAnnotationValue => null;

        public virtual bool IsWaypointValue => false;
        public virtual IndexedWaypointValue AsWaypointValue => null;

        public virtual bool IsInstanceValue => false;
        public virtual IndexedInstanceValue AsInstanceValue => null;

        public virtual bool IsHostValue => false;
        public virtual IndexedHostValue AsHostValue => null;

        public virtual bool IsPointRefValue => false;
        public virtual IndexedPointRefValue AsPointRefValue => null;

        public virtual bool IsRuleInstanceValue => false;
        public virtual IndexedRuleInstanceValue AsRuleInstanceValue => null;

        public virtual bool IsLogicalSearchResultValue => false;
        public virtual IndexedLogicalSearchResultValue AsLogicalSearchResultValue => null;

        public abstract object GetSystemValue();

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");

            sb.PrintBriefObjProp(n, nameof(OriginalValue), OriginalValue);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");

            sb.PrintBriefObjProp(n, nameof(OriginalValue), OriginalValue);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(KindOfValue)} = {KindOfValue}");

            sb.PrintBriefObjProp(n, nameof(OriginalValue), OriginalValue);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
