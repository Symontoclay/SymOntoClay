using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class PrimitiveTaskOperator : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public AstExpression Expression { get; set; }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public PrimitiveTaskOperator Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public PrimitiveTaskOperator Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (PrimitiveTaskOperator)context[this];
            }

            var result = new PrimitiveTaskOperator();
            context[this] = result;

            result.Expression = Expression.CloneAstExpression(context);

            return result;
        }

        public void DiscoverAllAnnotations(IList<Annotation> result)
        {
            Expression?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(Expression), Expression);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Expression), Expression);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);

            return sb.ToString();
        }

        public string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            if (Expression != null)
            {
                sb.Append(Expression.ToHumanizedString(options));
            }

            sb.Append(";");

            return sb.ToString();
        }

        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}
