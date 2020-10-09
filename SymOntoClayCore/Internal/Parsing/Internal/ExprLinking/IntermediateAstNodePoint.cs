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
    public class IntermediateAstNodePoint : IObjectToString
    {
        public IntermediateAstNode RootNode { get; set; }
        public IntermediateAstNode CurrentNode { get; set; }

        public T BuildExpr<T>() where T: IAstNode
        {
            return (T)BuildExpr();
        }

        public IAstNode BuildExpr()
        {
            return RootNode?.BuildExpr();
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
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(RootNode), RootNode);
            sb.PrintObjProp(n, nameof(CurrentNode), CurrentNode);

            return sb.ToString();
        }
    }
}
