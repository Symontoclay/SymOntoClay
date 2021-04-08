using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RangeValue : Value
    {
        public RangeBoundary LeftBoundary { get; set; }
        public RangeBoundary RightBoundary { get; set; }

        public double Length
        {
            get
            {
                if(LeftBoundary == null || RightBoundary == null)
                {
                    return double.PositiveInfinity;
                }

                return RightBoundary.Value.SystemValue.Value - LeftBoundary.Value.SystemValue.Value;
            }
        }

        public bool IsFit(NumberValue x)
        {
            if(x == null)
            {
                return false;
            }

            return IsFit(x.SystemValue);
        }

        public bool IsFit(double? x)
        {
            if(!x.HasValue)
            {
                return false;
            }

            var value = x.Value;

            if(LeftBoundary != null)
            {
                var leftValue = LeftBoundary.Value.SystemValue.Value;

                if(LeftBoundary.Includes)
                {
                    if(value < leftValue)
                    {
                        return false;
                    }
                }
                else
                {
                    if (value <= leftValue)
                    {
                        return false;
                    }
                }
            }

            if(RightBoundary != null)
            {
                var rightValue = RightBoundary.Value.SystemValue.Value;

                if(RightBoundary.Includes)
                {
                    if (value > rightValue)
                    {
                        return false;
                    }
                }
                else
                {
                    if(value >= rightValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.RangeValue;

        /// <inheritdoc/>
        public override bool IsRangeValue => true;

        /// <inheritdoc/>
        public override RangeValue AsRangeValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return this;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode();

            if(LeftBoundary != null)
            {
                LeftBoundary.CheckDirty();
                result ^= LeftBoundary.GetLongHashCode();
            }

            if (RightBoundary != null)
            {
                RightBoundary.CheckDirty();
                result ^= RightBoundary.GetLongHashCode();
            }

            return result;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public RangeValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public RangeValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RangeValue)context[this];
            }

            var result = new RangeValue();
            context[this] = result;

            result.LeftBoundary = LeftBoundary?.Clone(context);
            result.RightBoundary = RightBoundary?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(LeftBoundary), LeftBoundary);
            sb.PrintObjProp(n, nameof(RightBoundary), RightBoundary);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(LeftBoundary), LeftBoundary);
            sb.PrintShortObjProp(n, nameof(RightBoundary), RightBoundary);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(LeftBoundary), LeftBoundary);
            sb.PrintBriefObjProp(n, nameof(RightBoundary), RightBoundary);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            var sb = new StringBuilder();

            if (LeftBoundary == null)
            {
                sb.Append("(-∞");
            }
            else
            {
                if(LeftBoundary.Includes)
                {
                    sb.Append("[");
                }
                else
                {
                    sb.Append("(");
                }

                sb.Append(LeftBoundary.Value.GetSystemValue());
            }

            sb.Append(",");

            if(RightBoundary == null)
            {
                sb.Append("+∞)");
            }
            else
            {
                sb.Append(RightBoundary.Value.GetSystemValue());

                if (RightBoundary.Includes)
                {
                    sb.Append("]");
                }
                else
                {
                    sb.Append(")");
                }
            }

            return $"{spaces}{sb}";
        }
    }
}
