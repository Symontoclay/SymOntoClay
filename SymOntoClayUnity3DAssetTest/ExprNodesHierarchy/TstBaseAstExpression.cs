/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests.ExprNodesHierarchy
{
    public abstract class TstBaseAstExpression : IAstNode, IObjectToString
    {
        public abstract TstKindOfNode Kind { get; }

        protected virtual IAstNode NLeft { get => throw new NotImplementedException("2B329884-1ED0-4A64-8AC2-C07434536AAF"); set => throw new NotImplementedException("8394B144-06E6-4E8C-8647-2E4A007F9999"); }
        protected virtual IAstNode NRight { get => throw new NotImplementedException("6F02C4ED-ECF6-4F2E-9FE3-73945744F3A5"); set => throw new NotImplementedException("A8EDD3C5-0D09-40DD-AC42-D87BC838FACF"); }
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
            throw new NotImplementedException("4A3E1E20-3DE0-4511-8DD2-1B35CFF14EDA");
        }

        public virtual object Calc()
        {
            throw new NotImplementedException("E9484ED9-E594-42BF-B325-AAACDC193529");
        }
    }
}
