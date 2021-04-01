using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FuzzyLogicNonNumericSequenceValue : Value
    {
        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.FuzzyLogicNonNumericSequenceValue;

        /// <inheritdoc/>
        public override bool IsFuzzyLogicNonNumericSequenceValue => true;

        /// <inheritdoc/>
        public override FuzzyLogicNonNumericSequenceValue AsFuzzyLogicNonNumericSequenceValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
            //return NormalizedNameValue;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
            //return NormalizedNameValue.GetHashCode();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            throw new NotImplementedException();
            //return base.CalculateLongHashCode() ^ (ulong)NormalizedNameValue.GetHashCode();
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
        public StrongIdentifierValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public StrongIdentifierValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (StrongIdentifierValue)context[this];
            }

            var result = new StrongIdentifierValue();
            context[this] = result;

            throw new NotImplementedException();

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

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();


            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            //return $"{spaces}`{NameValue}`";

            throw new NotImplementedException();
        }
    }
}
