using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public class NewAstExpression : CallingFunctionAstExpression
    {
        /// <inheritdoc/>
        public override KindOfAstExpression Kind => KindOfAstExpression.New;

        /// <inheritdoc/>
        public override bool IsNewAstExpression => true;

        /// <inheritdoc/>
        public override NewAstExpression AsNewAstExpression => this;

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

            result.FillUpCallingFunctionAstExpression(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            //Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();
            sb.Append("new ");

            if (Left != null)
            {
                sb.Append(Left.ToHumanizedString(options));
            }

            if(!Parameters.IsNullOrEmpty())
            {
                var paramsStrList = new List<string>();

                foreach(var item in Parameters)
                {
                    paramsStrList.Add(item.ToHumanizedString(options));
                }

                sb.Append($"({string.Join(", ", paramsStrList)})");
            }

            return sb.ToString();
        }
    }
}
