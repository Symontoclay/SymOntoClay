using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public class CallingFunctionAstExpression : AstExpression
    {
        /// <inheritdoc/>
        public override KindOfAstExpression Kind => KindOfAstExpression.CallingFunction;

        public AstExpression Left { get; set; }

        /// <inheritdoc/>
        protected override IAstNode NLeft { get => Left; set => Left = (AstExpression)value; }

        public List<CallingParameter> MainParameters { get; set; } = new List<CallingParameter>();
        public List<CallingParameter> AdditionalParameters { get; set; } = new List<CallingParameter>();

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => null;

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext)
        {
            return null;
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            return null;
        }

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

            var result = new CallingFunctionAstExpression();
            context[this] = result;

            result.Left = Left.CloneAstExpression(context);

            result.MainParameters = MainParameters?.Select(p => p.Clone(context)).ToList();
            result.AdditionalParameters = AdditionalParameters?.Select(p => p.Clone(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(MainParameters), MainParameters);
            sb.PrintObjListProp(n, nameof(AdditionalParameters), AdditionalParameters);
            sb.PrintObjProp(n, nameof(Left), Left);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(MainParameters), MainParameters);
            sb.PrintShortObjListProp(n, nameof(AdditionalParameters), AdditionalParameters);
            sb.PrintShortObjProp(n, nameof(Left), Left);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(MainParameters), MainParameters);
            sb.PrintBriefObjListProp(n, nameof(AdditionalParameters), AdditionalParameters);
            sb.PrintBriefObjProp(n, nameof(Left), Left);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
