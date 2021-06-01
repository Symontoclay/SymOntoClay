using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ErrorValue : Value
    {
        public ErrorValue(RuleInstance ruleInstance)
        {
            RuleInstance = ruleInstance;
        }

        public RuleInstance RuleInstance { get; private set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ErrorValue;

        /// <inheritdoc/>
        public override bool IsErrorValue => true;

        /// <inheritdoc/>
        public override ErrorValue AsErrorValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode();

            RuleInstance.CheckDirty();

            result ^= RuleInstance.GetLongHashCode();

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            throw new NotImplementedException();
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
        public ErrorValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public ErrorValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (ErrorValue)context[this];
            }

            var result = new ErrorValue(RuleInstance.Clone(context));
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);

            //sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);

            //sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);

            //sb.AppendLine($"{spaces}{nameof(SystemValue)} = {SystemValue}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            //var spaces = DisplayHelper.Spaces(n);
            //return $"{spaces}{SystemValue}";

            throw new NotImplementedException();
        }
    }
}
