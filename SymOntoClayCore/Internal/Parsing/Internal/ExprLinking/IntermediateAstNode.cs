/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
