/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
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

        public List<CallingParameter> Parameters { get; set; } = new List<CallingParameter>();

        public bool IsAsync { get; set; }
        public bool IsChild { get; set; }

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

            result.FillUpCallingFunctionAstExpression(this, context);

            return result;
        }

        protected void FillUpCallingFunctionAstExpression(CallingFunctionAstExpression source, Dictionary<object, object> context)
        {
            Left = source.Left.CloneAstExpression(context);
            
            Parameters = source.Parameters?.Select(p => p.Clone(context)).ToList();

            IsAsync = source.IsAsync;
            IsChild = source.IsChild;

            AppendAnnotations(source, context);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            Left?.DiscoverAllAnnotations(result);

            if(!Parameters.IsNullOrEmpty())
            {
                foreach(var item in Parameters)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(Parameters), Parameters);
            sb.PrintObjProp(n, nameof(Left), Left);

            sb.AppendLine($"{spaces}{IsAsync} = {IsAsync}");
            sb.AppendLine($"{spaces}{IsChild} = {IsChild}");

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Parameters), Parameters);
            sb.PrintShortObjProp(n, nameof(Left), Left);

            sb.AppendLine($"{spaces}{IsAsync} = {IsAsync}");
            sb.AppendLine($"{spaces}{IsChild} = {IsChild}");

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Parameters), Parameters);
            sb.PrintBriefObjProp(n, nameof(Left), Left);

            sb.AppendLine($"{spaces}{IsAsync} = {IsAsync}");
            sb.AppendLine($"{spaces}{IsChild} = {IsChild}");

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            if(Left != null)
            {
                sb.Append(Left.ToHumanizedString(options));
            }

            if(IsAsync)
            {
                if(IsChild)
                {
                    sb.Append("~");
                }
                else
                {
                    sb.Append("~~");
                }                
            }

            sb.Append("(");

            if(!Parameters.IsNullOrEmpty())
            {
                var parametersStrList = new List<string>();

                foreach(var parameter in Parameters)
                {
                    parametersStrList.Add(parameter.ToHumanizedString(options));
                }

                sb.Append(string.Join(", ", parametersStrList));
            }

            sb.Append(")");
            
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }
    }
}
