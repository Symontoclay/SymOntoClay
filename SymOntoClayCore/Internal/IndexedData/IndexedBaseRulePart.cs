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

using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    [Obsolete("IndexedData must be removed!", true)]
    public abstract class IndexedBaseRulePart: IndexedAnnotatedItem
    {
        public abstract BaseRulePart OriginRulePart { get; }

        /// <inheritdoc/>
        public override AnnotatedItem OriginalAnnotatedItem => OriginRulePart;

        public IndexedRuleInstance Parent { get; set; }

        public bool IsActive { get; set; }
        public bool HasVars { get; set; }
        public bool HasQuestionVars { get; set; }

        public BaseIndexedLogicalQueryNode Expression { get; set; }

        public IDictionary<ulong, IList<RelationIndexedLogicalQueryNode>> RelationsDict { get; set; }

        public abstract IList<IndexedBaseRulePart> GetNextPartsList();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            return base.CalculateLongHashCode() ^ Expression.GetLongHashCode();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            var nextN = n + 4;
            var nextNSpace = DisplayHelper.Spaces(nextN);

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");

            sb.PrintBriefObjProp(n, nameof(OriginRulePart), OriginRulePart);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);

            sb.PrintObjProp(n, nameof(Expression), Expression);

            if (RelationsDict == null)
            {
                sb.AppendLine($"{spaces}{nameof(RelationsDict)} = null");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(RelationsDict)}");
                var nextNextN = nextN + 4;
                foreach (var relationsKVPItem in RelationsDict)
                {
                    sb.AppendLine($"{nextNSpace}key of relation = {relationsKVPItem.Key}");
                    var tmpRelationsList = relationsKVPItem.Value;
                    sb.AppendLine($"{nextNSpace}count of relations = {tmpRelationsList.Count}");
                    foreach (var relation in tmpRelationsList)
                    {
                        sb.Append(relation.ToShortString(nextNextN));
                    }
                }
                sb.AppendLine($"{spaces}End {nameof(RelationsDict)}");
            }

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");

            sb.PrintBriefObjProp(n, nameof(OriginRulePart), OriginRulePart);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.PrintShortObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsActive)} = {IsActive}");
            sb.AppendLine($"{spaces}{nameof(HasVars)} = {HasVars}");
            sb.AppendLine($"{spaces}{nameof(HasQuestionVars)} = {HasQuestionVars}");

            sb.PrintBriefObjProp(n, nameof(OriginRulePart), OriginRulePart);
            sb.PrintBriefObjProp(n, nameof(Parent), Parent);
            sb.PrintBriefObjProp(n, nameof(Expression), Expression);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public void CalculateUsedKeys(List<ulong> usedKeysList)
        {
            Expression.CalculateUsedKeys(usedKeysList);
        }
    }
}
