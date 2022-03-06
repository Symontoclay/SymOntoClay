using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstIfStatement : AstStatement
    {
        public AstExpression ConditionalExpression { get; set; }

        public List<AstStatement> IfStatements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.IfStatement;

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

            var result = new AstIfStatement();
            context[this] = result;

            result.ConditionalExpression = ConditionalExpression.CloneAstExpression(context);

            result.IfStatements = IfStatements?.Select(p => p.CloneAstStatement(context)).ToList();
            //result.CatchStatements = CatchStatements?.Select(p => p.Clone(context)).ToList();
            //result.ElseStatements = ElseStatements?.Select(p => p.CloneAstStatement(context)).ToList();
            //result.EnsureStatements = EnsureStatements?.Select(p => p.CloneAstStatement(context)).ToList();

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
        public override void CalculateLongHashCodes(CheckDirtyOptions options)
        {
            ConditionalExpression.CheckDirty(options);

            if (IfStatements.IsNullOrEmpty())
            {
                foreach (var statement in IfStatements)
                {
                    statement.CheckDirty(options);
                }
            }

            //if (CatchStatements.IsNullOrEmpty())
            //{
            //    foreach (var statement in CatchStatements)
            //    {
            //        statement.CheckDirty(options);
            //    }
            //}

            //if (ElseStatements.IsNullOrEmpty())
            //{
            //    foreach (var statement in ElseStatements)
            //    {
            //        statement.CheckDirty(options);
            //    }
            //}

            //if (EnsureStatements.IsNullOrEmpty())
            //{
            //    foreach (var statement in EnsureStatements)
            //    {
            //        statement.CheckDirty(options);
            //    }
            //}

            base.CalculateLongHashCodes(options);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(ConditionalExpression), ConditionalExpression);

            sb.PrintObjListProp(n, nameof(IfStatements), IfStatements);
            //sb.PrintObjListProp(n, nameof(CatchStatements), CatchStatements);
            //sb.PrintObjListProp(n, nameof(ElseStatements), ElseStatements);
            //sb.PrintObjListProp(n, nameof(EnsureStatements), EnsureStatements);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(ConditionalExpression), ConditionalExpression);

            sb.PrintShortObjListProp(n, nameof(IfStatements), IfStatements);
            //sb.PrintShortObjListProp(n, nameof(CatchStatements), CatchStatements);
            //sb.PrintShortObjListProp(n, nameof(ElseStatements), ElseStatements);
            //sb.PrintShortObjListProp(n, nameof(EnsureStatements), EnsureStatements);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(ConditionalExpression), ConditionalExpression);

            sb.PrintBriefObjListProp(n, nameof(IfStatements), IfStatements);
            //sb.PrintBriefObjListProp(n, nameof(CatchStatements), CatchStatements);
            //sb.PrintBriefObjListProp(n, nameof(ElseStatements), ElseStatements);
            //sb.PrintBriefObjListProp(n, nameof(EnsureStatements), EnsureStatements);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
