/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Unity3DAsset.Test.ExprNodesHierarchy
{
    public abstract class TstBaseAstExpression : IAstNode, IObjectToString
    {
        public abstract TstKindOfNode Kind { get; }

        protected virtual IAstNode NLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected virtual IAstNode NRight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IAstNode IAstNode.Left { get => NLeft; set => NLeft = value; }
        IAstNode IAstNode.Right { get => NRight; set => NRight = value; }

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

            return sb.ToString();
        }

        public virtual string GetDbgString()
        {
            throw new NotImplementedException();
        }

        public virtual object Calc()
        {
            throw new NotImplementedException();
        }
    }
}
