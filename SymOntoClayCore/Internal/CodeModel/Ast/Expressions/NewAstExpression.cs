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
using SymOntoClay.Core.DebugHelpers;
using System.Collections.Generic;
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
