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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchOptions: ResolverOptions
    {
        public bool EntityIdOnly { get; set; }
        public bool IgnoreAccessPolicy { get; set; } = true;
        public bool UseInheritance { get; set; } = true;
        public IndexedRuleInstance QueryExpression { get; set; }
        public LocalCodeExecutionContext LocalCodeExecutionContext { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(EntityIdOnly)} = {EntityIdOnly}");
            sb.AppendLine($"{spaces}{nameof(IgnoreAccessPolicy)} = {IgnoreAccessPolicy}");
            sb.AppendLine($"{spaces}{nameof(UseInheritance)} = {UseInheritance}");

            sb.PrintObjProp(n, nameof(QueryExpression), QueryExpression);
            sb.PrintObjProp(n, nameof(LocalCodeExecutionContext), LocalCodeExecutionContext);

            //sb.PrintObjProp(n, nameof(), );

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(EntityIdOnly)} = {EntityIdOnly}");
            sb.AppendLine($"{spaces}{nameof(IgnoreAccessPolicy)} = {IgnoreAccessPolicy}");
            sb.AppendLine($"{spaces}{nameof(UseInheritance)} = {UseInheritance}");

            sb.PrintShortObjProp(n, nameof(QueryExpression), QueryExpression);
            sb.PrintShortObjProp(n, nameof(LocalCodeExecutionContext), LocalCodeExecutionContext);

            //sb.PrintShortObjProp(n, nameof(), );

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(EntityIdOnly)} = {EntityIdOnly}");
            sb.AppendLine($"{spaces}{nameof(IgnoreAccessPolicy)} = {IgnoreAccessPolicy}");
            sb.AppendLine($"{spaces}{nameof(UseInheritance)} = {UseInheritance}");

            sb.PrintBriefObjProp(n, nameof(QueryExpression), QueryExpression);
            sb.PrintBriefObjProp(n, nameof(LocalCodeExecutionContext), LocalCodeExecutionContext);

            //sb.PrintBriefObjProp(n, nameof(), );

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
