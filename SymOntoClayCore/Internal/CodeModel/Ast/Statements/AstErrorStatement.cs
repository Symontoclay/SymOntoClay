using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstErrorStatement : AstStatement
    {
        public RuleInstanceValue RuleInstanceValue { get; set; }

        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.ErrorStatement;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneAstStatement(context);
        }

        /// <inheritdoc/>
        public override AstStatement CloneAstStatement(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstStatement)context[this];
            }

            var result = new AstErrorStatement();
            context[this] = result;

            result.RuleInstanceValue = RuleInstanceValue?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void CalculateLongHashCodes()
        {
            RuleInstanceValue.CheckDirty();

            base.CalculateLongHashCodes();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(RuleInstanceValue), RuleInstanceValue);
            //sb.PrintObjProp(n, nameof(SuperName), SuperName);
            //sb.PrintObjProp(n, nameof(Rank), Rank);

            //sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(RuleInstanceValue), RuleInstanceValue);
            //sb.PrintShortObjProp(n, nameof(SuperName), SuperName);
            //sb.PrintShortObjProp(n, nameof(Rank), Rank);

            //sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(RuleInstanceValue), RuleInstanceValue);
            //sb.PrintBriefObjProp(n, nameof(SuperName), SuperName);
            //sb.PrintBriefObjProp(n, nameof(Rank), Rank);

            //sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
