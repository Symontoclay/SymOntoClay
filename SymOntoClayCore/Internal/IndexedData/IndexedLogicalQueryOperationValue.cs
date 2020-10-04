using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedLogicalQueryOperationValue : IndexedValue
    {
        public KindOfLogicalQueryOperation KindOfLogicalQueryOperation { get; set; } = KindOfLogicalQueryOperation.Unknown;

        /// <summary>
        /// Inserted or searched value.
        /// </summary>
        public IndexedValue Target { get; set; }

        public IndexedValue Source { get; set; }
        public IndexedValue Dest { get; set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.LogicalQueryOperationValue;

        /// <inheritdoc/>
        public override bool IsLogicalQueryOperationValue => true;

        /// <inheritdoc/>
        public override IndexedLogicalQueryOperationValue AsLogicalQueryOperationValue => this;

        public LogicalQueryOperationValue OriginalLogicalQueryOperationValue { get; set; }

        /// <inheritdoc/>
        public override Value OriginalValue => OriginalLogicalQueryOperationValue;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return this;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode() ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfLogicalQueryOperation.GetHashCode());

            if(Target != null)
            {
                result ^= Target.GetLongHashCode();
            }

            if (Source != null)
            {
                result ^= Source.GetLongHashCode();
            }

            if (Dest != null)
            {
                result ^= Dest.GetLongHashCode();
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfLogicalQueryOperation)} = {KindOfLogicalQueryOperation}");

            sb.PrintObjProp(n, nameof(Target), Target);
            sb.PrintObjProp(n, nameof(Source), Source);
            sb.PrintObjProp(n, nameof(Dest), Dest);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfLogicalQueryOperation)} = {KindOfLogicalQueryOperation}");

            sb.PrintShortObjProp(n, nameof(Target), Target);
            sb.PrintShortObjProp(n, nameof(Source), Source);
            sb.PrintShortObjProp(n, nameof(Dest), Dest);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfLogicalQueryOperation)} = {KindOfLogicalQueryOperation}");

            sb.PrintBriefObjProp(n, nameof(Target), Target);
            sb.PrintBriefObjProp(n, nameof(Source), Source);
            sb.PrintBriefObjProp(n, nameof(Dest), Dest);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.Append(spaces);

            switch (KindOfLogicalQueryOperation)
            {
                case KindOfLogicalQueryOperation.Select:
                    sb.Append("SELECT ");
                    break;

                case KindOfLogicalQueryOperation.Insert:
                    sb.Append("INSERT ");
                    break;

                case KindOfLogicalQueryOperation.Delete:
                    sb.Append("DELETE ");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfLogicalQueryOperation), KindOfLogicalQueryOperation, null);
            }

            if(Target != null)
            {
                switch(Target.KindOfValue)
                {
                    case KindOfValue.RuleInstanceValue:
                        {
                            var val = Target.AsRuleInstanceValue;

                            sb.Append(val.IndexedRuleInstance.GetDefaultToDbgStringInformation(0u));
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Target.KindOfValue), Target.KindOfValue, null);
                }
            }

            if (Source != null)
            {
                switch (Source.KindOfValue)
                {
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Source.KindOfValue), Source.KindOfValue, null);
                }
            }

            if (Dest != null)
            {
                switch (Dest.KindOfValue)
                {
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Dest.KindOfValue), Dest.KindOfValue, null);
                }
            }

            return sb.ToString();
        }
    }
}
