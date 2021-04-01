using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FuzzyLogicNonNumericValue : AnnotatedItem
    {
        public StrongIdentifierValue Name { get; set; }
        public IFuzzyLogicMemberFunctionHandler Handler { get; set; }
        public LinguisticVariable Parent { get; set; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public FuzzyLogicNonNumericValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public FuzzyLogicNonNumericValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (FuzzyLogicNonNumericValue)context[this];
            }

            var result = new FuzzyLogicNonNumericValue();
            context[this] = result;

            result.Name = Name.Clone(context);
            result.Handler = Handler;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Name), Name);
            sb.PrintObjProp(n, nameof(Handler), Handler);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.PrintShortObjProp(n, nameof(Handler), Handler);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.PrintBriefObjProp(n, nameof(Handler), Handler);
            sb.PrintExisting(n, nameof(Parent), Parent);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
