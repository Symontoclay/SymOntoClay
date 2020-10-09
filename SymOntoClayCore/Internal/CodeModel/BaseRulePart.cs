/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseRulePart: AnnotatedItem
    {
        public RuleInstance Parent { get; set; }
        public bool IsActive { get; set; }
        public LogicalQueryNode Expression { get; set; }

        public bool HasQuestionVars
        {
            get
            {
                if(Expression == null)
                {
                    return false;
                }

                return Expression.HasQuestionVars;
            }
        }

        protected void AppendBaseRulePart(BaseRulePart source, Dictionary<object, object> context)
        {
            IsActive = source.IsActive;
            Parent = source.Parent.Clone(context);
            Expression = source.Expression.Clone(context);

            AppendAnnotations(source, context);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Expression?.DiscoverAllAnnotations(result);
        }

        public IList<LogicalQueryNode> GetInheritanceRelations()
        {
            var result = new List<LogicalQueryNode>();
            Expression.DiscoverAllInheritanceRelations(result);
            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");

            sb.PrintObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");

            sb.PrintShortObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
