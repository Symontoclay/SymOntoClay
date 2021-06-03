using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstCatchStatement : AstStatement
    {
        public StrongIdentifierValue VariableName { get; set; }
        public RuleInstance Condition { get; set; }
        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.CatchStatement;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override AstStatement CloneAstStatement(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public AstCatchStatement Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public AstCatchStatement Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstCatchStatement)context[this];
            }

            var result = new AstCatchStatement();
            context[this] = result;

            result.VariableName = VariableName?.Clone(context);
            result.Condition = Condition?.Clone(context);
            result.Statements = Statements?.Select(p => p.CloneAstStatement(context)).ToList();

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
            //RuleInstanceValue.CheckDirty();

            base.CalculateLongHashCodes();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(VariableName), VariableName);
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintObjListProp(n, nameof(Statements), Statements);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(VariableName), VariableName);
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintShortObjListProp(n, nameof(Statements), Statements);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(VariableName), VariableName);
            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
