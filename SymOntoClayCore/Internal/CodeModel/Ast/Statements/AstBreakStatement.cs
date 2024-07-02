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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstBreakStatement : AstStatement
    {
        /// <inheritdoc/>
        public override KindOfAstStatement Kind => KindOfAstStatement.BreakStatement;

        public KindOfBreak KindOfBreak { get; set; } = KindOfBreak.Unknown;
        public RuleInstance RuleInstance { get; set; }

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

            var result = new AstBreakStatement();
            context[this] = result;

            result.KindOfBreak = KindOfBreak;
            result.RuleInstance = RuleInstance?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void CalculateLongHashCodes(CheckDirtyOptions options)
        {
            RuleInstance?.CheckDirty(options);

            base.CalculateLongHashCodes(options);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfBreak)} = {KindOfBreak}");
            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfBreak)} = {KindOfBreak}");
            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfBreak)} = {KindOfBreak}");
            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("D6EB6FBD-DF85-46A7-BABC-F7C9C8D8B533");
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException("BC3FB2ED-015B-44FE-AE0F-4E64C3FE72BF");
        }
    }
}
