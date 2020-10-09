/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
