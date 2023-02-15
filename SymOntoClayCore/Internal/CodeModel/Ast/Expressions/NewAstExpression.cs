using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public class NewAstExpression : AstExpression
    {
        /// <inheritdoc/>
        public override KindOfAstExpression Kind => KindOfAstExpression.New;

        /// <inheritdoc/>
        public override bool IsNewAstExpression => true;

        /// <inheritdoc/>
        public override NewAstExpression AsNewAstExpression => this;

        public AstExpression PrototypeExpression { get; set; }

        public List<CallingParameter> Parameters { get; set; } = new List<CallingParameter>();

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneAstExpression(context);
        }

        /// <inheritdoc/>
        public override AstExpression CloneAstExpression(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstExpression)context[this];
            }

            var result = new NewAstExpression();
            context[this] = result;

            result.PrototypeExpression = PrototypeExpression.CloneAstExpression(context);
            result.Parameters = Parameters?.Select(p => p.Clone(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            //Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(PrototypeExpression), PrototypeExpression);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(PrototypeExpression), PrototypeExpression);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(PrototypeExpression), PrototypeExpression);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();
            sb.Append("new ");

            if (PrototypeExpression != null)
            {
                sb.Append(PrototypeExpression.ToHumanizedString(options));
            }

            if(!Parameters.IsNullOrEmpty())
            {
                var paramsStrList = new List<string>();

                foreach(var item in Parameters)
                {
                    paramsStrList.Add(item.ToHumanizedString(options));
                }

                sb.Append($"({string.Join(', ', paramsStrList)})");
            }

            return sb.ToString();
        }
    }
}
