using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstWaitStatement : AstStatement
    {
        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.WaitStatement;

        public List<AstExpression> Items { get; set; } = new List<AstExpression>();

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

            var result = new AstWaitStatement();
            context[this] = result;

            result.Items = Items?.Select(p => p.CloneAstExpression(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            if(!Items.IsNullOrEmpty())
            {
                foreach(var item in Items)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        public override void CalculateLongHashCodes(CheckDirtyOptions options)
        {
            if (!Items.IsNullOrEmpty())
            {
                foreach (var item in Items)
                {
                    item.CheckDirty(options);
                }
            }

            base.CalculateLongHashCodes(options);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(Items), Items);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Items), Items);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Items), Items);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return $"wait {string.Join(",", Items?.Select(p => p.ToHumanizedString(options)))};";
        }
    }
}
