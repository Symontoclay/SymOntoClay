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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseHtnTask : CodeItem
    {
        /// <inheritdoc/>
        public override bool IsBaseHtnTask => true;

        /// <inheritdoc/>
        public override BaseHtnTask AsBaseHtnTask => this;

        public abstract BaseHtnTask CloneBaseTask(Dictionary<object, object> context);

        public abstract KindOfTask KindOfTask { get; }

        public LogicalExecutableExpression Precondition { get; set; }
        public AstExpression PreconditionExpression { get; set; }

        protected void AppendBaseHtnTask(BaseHtnTask source, Dictionary<object, object> context)
        {
            Precondition = source.Precondition?.Clone(context);
            PreconditionExpression = source.PreconditionExpression?.CloneAstExpression(context);

            AppendCodeItem(source, context);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            Precondition.CheckDirty(options);

            var result = base.CalculateLongHashCode(options);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");
            sb.PrintObjProp(n, nameof(Precondition), Precondition);
            sb.PrintObjProp(n, nameof(PreconditionExpression), PreconditionExpression);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");
            sb.PrintShortObjProp(n, nameof(Precondition), Precondition);
            sb.PrintShortObjProp(n, nameof(PreconditionExpression), PreconditionExpression);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");
            sb.PrintBriefObjProp(n, nameof(Precondition), Precondition);
            sb.PrintBriefObjProp(n, nameof(PreconditionExpression), PreconditionExpression);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
