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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchExplainNode: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        public LogicalSearchExplainNode(LogicalSearchExplainNode rootParent)
        {
            RootParent = rootParent;
            
            if(rootParent != null)
            {
                var commonChildren = rootParent.CommonChildren;

                if (!commonChildren.Contains(this))
                {
                    commonChildren.Add(this);
                }
            }
        }

        public KindOfLogicalSearchExplainNode Kind { get; set; } = KindOfLogicalSearchExplainNode.Unknown;
        public LogicalSearchExplainNode Parent { get; set; }
        public List<LogicalSearchExplainNode> Children { get; set; } = new List<LogicalSearchExplainNode>();
        public LogicalSearchExplainNode RootParent { get; set; }
        public List<LogicalSearchExplainNode> CommonChildren { get; set; } = new List<LogicalSearchExplainNode>();
        public RuleInstance ProcessedRuleInstance { get; set; }
        public PrimaryRulePart ProcessedPrimaryRulePart { get; set; }
        public LogicalQueryNode ProcessedLogicalQueryNode { get; set; }
        public BaseRulePart ProcessedBaseRulePart { get; set; }
        public StrongIdentifierValue TargetRelation { get; set; }
        public KindOfOperatorOfLogicalQueryNode KindOfOperator { get; set; } = KindOfOperatorOfLogicalQueryNode.Unknown;
        public List<string> AdditionalInformation { get; set; } = new List<string>();
        public bool IsFit { get; set; }
        public bool IsSuccess { get; set; }
        public List<ResultOfQueryToRelation> ResultsOfQueryToRelationList { get; set; }
        public IList<BaseRulePart> BaseRulePartList { get; set; }
        public IList<LogicalQueryNode> RelationsList  { get; set; }
        public StrongIdentifierValue Key { get; set; }
        public ILogicalStorage LogicalStorage { get; set; }
        public string StorageName { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }

        public static void LinkNodes(LogicalSearchExplainNode parent, LogicalSearchExplainNode child)
        {
            var parentChildren = parent.Children;

            if (!parentChildren.Contains(child))
            {
                parentChildren.Add(child);
            }
            
            child.Parent = parent;
        }

        public static void ResetParent(LogicalSearchExplainNode child)
        {
            var parent = child.Parent;

            child.Parent = null;

            if(parent != null)
            {
                parent.Children.Remove(child);
            }
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintObjListProp(n, nameof(Children), Children);
            sb.PrintObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.PrintObjProp(n, nameof(ProcessedLogicalQueryNode), ProcessedLogicalQueryNode);
            sb.PrintObjProp(n, nameof(ProcessedBaseRulePart), ProcessedBaseRulePart);            
            sb.PrintObjProp(n, nameof(TargetRelation), TargetRelation);
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
            sb.PrintPODList(n, nameof(AdditionalInformation), AdditionalInformation);
            sb.AppendLine($"{spaces}{nameof(IsFit)} = {IsFit}");
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintObjListProp(n, nameof(BaseRulePartList), BaseRulePartList);
            sb.PrintObjListProp(n, nameof(RelationsList), RelationsList);
            sb.PrintObjProp(n, nameof(Key), Key);
            sb.PrintExisting(n, nameof(LogicalStorage), LogicalStorage);
            sb.AppendLine($"{spaces}{nameof(StorageName)} = {StorageName}");
            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintBriefObjListProp(n, nameof(Children), Children);
            sb.PrintShortObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintShortObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.PrintShortObjProp(n, nameof(ProcessedLogicalQueryNode), ProcessedLogicalQueryNode);
            sb.PrintShortObjProp(n, nameof(ProcessedBaseRulePart), ProcessedBaseRulePart);
            sb.PrintShortObjProp(n, nameof(TargetRelation), TargetRelation);
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
            sb.PrintPODList(n, nameof(AdditionalInformation), AdditionalInformation);
            sb.AppendLine($"{spaces}{nameof(IsFit)} = {IsFit}");
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintShortObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintShortObjListProp(n, nameof(BaseRulePartList), BaseRulePartList);
            sb.PrintShortObjListProp(n, nameof(RelationsList), RelationsList);
            sb.PrintShortObjProp(n, nameof(Key), Key);
            sb.PrintExisting(n, nameof(LogicalStorage), LogicalStorage);
            sb.AppendLine($"{spaces}{nameof(StorageName)} = {StorageName}");
            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintExisting(n, nameof(Children), Children);
            sb.PrintBriefObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintBriefObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.PrintBriefObjProp(n, nameof(ProcessedLogicalQueryNode), ProcessedLogicalQueryNode);
            sb.PrintBriefObjProp(n, nameof(ProcessedBaseRulePart), ProcessedBaseRulePart);
            sb.PrintBriefObjProp(n, nameof(TargetRelation), TargetRelation);
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
            sb.PrintPODList(n, nameof(AdditionalInformation), AdditionalInformation);
            sb.AppendLine($"{spaces}{nameof(IsFit)} = {IsFit}");
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintExisting(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintExisting(n, nameof(BaseRulePartList), BaseRulePartList);
            sb.PrintExisting(n, nameof(RelationsList), RelationsList);
            sb.PrintBriefObjProp(n, nameof(Key), Key);
            sb.PrintExisting(n, nameof(LogicalStorage), LogicalStorage);
            sb.AppendLine($"{spaces}{nameof(StorageName)} = {StorageName}");
            sb.PrintExisting(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintExisting(n, nameof(KnownInfoList), KnownInfoList);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintExisting(n, nameof(Children), Children);
            sb.PrintDbgObjProp(n, nameof(ProcessedRuleInstance), ProcessedRuleInstance);
            sb.PrintDbgObjProp(n, nameof(ProcessedPrimaryRulePart), ProcessedPrimaryRulePart);
            sb.PrintDbgObjProp(n, nameof(ProcessedLogicalQueryNode), ProcessedLogicalQueryNode);
            sb.PrintDbgObjProp(n, nameof(ProcessedBaseRulePart), ProcessedBaseRulePart);
            sb.PrintDbgObjProp(n, nameof(TargetRelation), TargetRelation);
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");
            sb.PrintPODList(n, nameof(AdditionalInformation), AdditionalInformation);
            sb.AppendLine($"{spaces}{nameof(IsFit)} = {IsFit}");
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");
            sb.PrintExisting(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.PrintExisting(n, nameof(BaseRulePartList), BaseRulePartList);
            sb.PrintExisting(n, nameof(RelationsList), RelationsList);
            sb.PrintBriefObjProp(n, nameof(Key), Key);
            sb.PrintExisting(n, nameof(LogicalStorage), LogicalStorage);
            sb.AppendLine($"{spaces}{nameof(StorageName)} = {StorageName}");
            sb.PrintExisting(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintExisting(n, nameof(KnownInfoList), KnownInfoList);

            return sb.ToString();
        }
    }
}
