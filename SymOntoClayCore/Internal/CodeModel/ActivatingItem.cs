using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ActivatingItem : AnnotatedItem
    {
        public RuleInstance Condition { get; set; }
        public BindingVariables BindingVariables { get; set; } = new BindingVariables();

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public ActivatingItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public ActivatingItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (ActivatingItem)context[this];
            }

            var result = new ActivatingItem();
            context[this] = result;

            result.Condition = Condition?.Clone(context);
            result.BindingVariables = BindingVariables.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintObjProp(n, nameof(BindingVariables), BindingVariables);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintShortObjProp(n, nameof(BindingVariables), BindingVariables);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintBriefObjProp(n, nameof(BindingVariables), BindingVariables);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
