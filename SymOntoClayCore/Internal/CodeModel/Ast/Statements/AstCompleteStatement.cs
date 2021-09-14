using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstCompleteStatement : AstStatement
    {
        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.CompleteStatement;

        public KindOfCompletion KindOfCompletion { get; set; } = KindOfCompletion.Unknown;
        public RuleInstanceValue RuleInstanceValue { get; set; }

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

            var result = new AstCompleteStatement();
            context[this] = result;

            result.KindOfCompletion = KindOfCompletion;
            result.RuleInstanceValue = RuleInstanceValue?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void CalculateLongHashCodes()
        {
            RuleInstanceValue?.CheckDirty();

            base.CalculateLongHashCodes();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfCompletion)} = {KindOfCompletion}");
            sb.PrintObjProp(n, nameof(RuleInstanceValue), RuleInstanceValue);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfCompletion)} = {KindOfCompletion}");
            sb.PrintShortObjProp(n, nameof(RuleInstanceValue), RuleInstanceValue);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfCompletion)} = {KindOfCompletion}");
            sb.PrintBriefObjProp(n, nameof(RuleInstanceValue), RuleInstanceValue);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
