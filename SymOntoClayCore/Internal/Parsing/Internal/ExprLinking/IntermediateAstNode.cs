/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking
{
    public class IntermediateAstNode : IObjectToString
    {
        public IntermediateAstNode(IAstNode astNode)
        {
            _astNode = astNode;
            Kind = KindOfIntermediateAstNode.Leaf;
            Priority = 0;
        }

        public IntermediateAstNode(IAstNode astNode, KindOfIntermediateAstNode kind, int priority)
        {
            _astNode = astNode;
            Kind = kind;
            Priority = priority;
        }

        public KindOfIntermediateAstNode Kind { get; private set; }
        public int Priority { get; private set; }

        private IAstNode _astNode;

        public IntermediateAstNode Parent { get; set; }
        public IntermediateAstNode Left { get; set; }
        public IntermediateAstNode Right { get; set; }

        public IAstNode BuildExpr()
        {
            switch (Kind)
            {
                case KindOfIntermediateAstNode.BinaryOperator:
                    _astNode.Left = Left?.BuildExpr();
                    _astNode.Right = Right?.BuildExpr();
                    break;

                case KindOfIntermediateAstNode.UnaryOperator:
                    _astNode.Left = Left?.BuildExpr();
                    break;

                case KindOfIntermediateAstNode.Leaf:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null);
            }

            return _astNode;
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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(Priority)} = {Priority}");

            sb.PrintObjProp(n, nameof(_astNode), _astNode);

            sb.PrintExisting(n, nameof(Parent), Parent);

            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Right), Right);

            return sb.ToString();
        }
    }
}
