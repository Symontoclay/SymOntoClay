/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstTryStatement : AstStatement
    {
        public List<AstStatement> TryStatements { get; set; } = new List<AstStatement>();
        public List<AstCatchStatement> CatchStatements { get; set; } = new List<AstCatchStatement>();
        public List<AstStatement> ElseStatements { get; set; } = new List<AstStatement>();
        public List<AstStatement> EnsureStatements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.TryStatement;

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneAstStatement(context);
        }

        /// <inheritdoc/>
        public override AstStatement CloneAstStatement(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstStatement)context[this];
            }

            var result = new AstTryStatement();
            context[this] = result;

            result.TryStatements = TryStatements?.Select(p => p.CloneAstStatement(context)).ToList();
            result.CatchStatements = CatchStatements?.Select(p => p.Clone(context)).ToList();
            result.ElseStatements = ElseStatements?.Select(p => p.CloneAstStatement(context)).ToList();
            result.EnsureStatements = EnsureStatements?.Select(p => p.CloneAstStatement(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void CalculateLongHashCodes(CheckDirtyOptions options)
        {
            if(TryStatements.IsNullOrEmpty())
            {
                foreach(var statement in TryStatements)
                {
                    statement.CheckDirty(options);
                }
            }

            if (CatchStatements.IsNullOrEmpty())
            {
                foreach (var statement in CatchStatements)
                {
                    statement.CheckDirty(options);
                }
            }

            if (ElseStatements.IsNullOrEmpty())
            {
                foreach (var statement in ElseStatements)
                {
                    statement.CheckDirty(options);
                }
            }

            if (EnsureStatements.IsNullOrEmpty())
            {
                foreach (var statement in EnsureStatements)
                {
                    statement.CheckDirty(options);
                }
            }

            base.CalculateLongHashCodes(options);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(TryStatements), TryStatements);
            sb.PrintObjListProp(n, nameof(CatchStatements), CatchStatements);
            sb.PrintObjListProp(n, nameof(ElseStatements), ElseStatements);
            sb.PrintObjListProp(n, nameof(EnsureStatements), EnsureStatements);
            
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(TryStatements), TryStatements);
            sb.PrintShortObjListProp(n, nameof(CatchStatements), CatchStatements);
            sb.PrintShortObjListProp(n, nameof(ElseStatements), ElseStatements);
            sb.PrintShortObjListProp(n, nameof(EnsureStatements), EnsureStatements);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(TryStatements), TryStatements);
            sb.PrintBriefObjListProp(n, nameof(CatchStatements), CatchStatements);
            sb.PrintBriefObjListProp(n, nameof(ElseStatements), ElseStatements);
            sb.PrintBriefObjListProp(n, nameof(EnsureStatements), EnsureStatements);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
