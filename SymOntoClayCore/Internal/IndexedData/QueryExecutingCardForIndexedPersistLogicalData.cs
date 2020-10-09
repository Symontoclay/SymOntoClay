/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class QueryExecutingCardForIndexedPersistLogicalData : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public ulong TargetRelation { get; set; }
        public int CountParams { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }
        public IList<ResultOfQueryToRelation> ResultsOfQueryToRelationList { get; set; } = new List<ResultOfQueryToRelation>();
        public bool IsSuccess { get; set; }

#if DEBUG
        public IndexedRuleInstance SenderIndexedRuleInstance { get; set; }
        public IndexedBaseRulePart SenderIndexedRulePart { get; set; }
        public BaseIndexedLogicalQueryNode SenderExpressionNode { get; set; }
#endif
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

            sb.AppendLine($"{spaces}{nameof(TargetRelation)} = {TargetRelation}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);
            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");

#if DEBUG
            sb.PrintExisting(n, nameof(SenderIndexedRuleInstance), SenderIndexedRuleInstance);
            sb.PrintExisting(n, nameof(SenderIndexedRulePart), SenderIndexedRulePart);
            sb.PrintExisting(n, nameof(SenderExpressionNode), SenderExpressionNode);
#endif
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

            sb.AppendLine($"{spaces}{nameof(TargetRelation)} = {TargetRelation}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintShortObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");

#if DEBUG
            sb.PrintExisting(n, nameof(SenderIndexedRuleInstance), SenderIndexedRuleInstance);
            sb.PrintExisting(n, nameof(SenderIndexedRulePart), SenderIndexedRulePart);
            sb.PrintExisting(n, nameof(SenderExpressionNode), SenderExpressionNode);
#endif
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

            sb.AppendLine($"{spaces}{nameof(TargetRelation)} = {TargetRelation}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.PrintBriefObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintBriefObjListProp(n, nameof(KnownInfoList), KnownInfoList);
            sb.PrintBriefObjListProp(n, nameof(ResultsOfQueryToRelationList), ResultsOfQueryToRelationList);

            sb.AppendLine($"{spaces}{nameof(IsSuccess)} = {IsSuccess}");

#if DEBUG
            sb.PrintExisting(n, nameof(SenderIndexedRuleInstance), SenderIndexedRuleInstance);
            sb.PrintExisting(n, nameof(SenderIndexedRulePart), SenderIndexedRulePart);
            sb.PrintExisting(n, nameof(SenderExpressionNode), SenderExpressionNode);
#endif
            return sb.ToString();
        }

#if DEBUG
        public string GetSenderIndexedRuleInstanceHumanizeDbgString()
        {
            if (SenderIndexedRuleInstance == null)
            {
                return string.Empty;
            }

            var origin = SenderIndexedRuleInstance.Origin;

            if (origin == null)
            {
                return string.Empty;
            }

            return DebugHelperForRuleInstance.ToString(origin);
        }

        public string GetSenderIndexedRulePartHumanizeDbgString()
        {
            if (SenderIndexedRulePart == null)
            {
                return string.Empty;
            }

            var origin = SenderIndexedRulePart.OriginRulePart;

            if (origin == null)
            {
                return string.Empty;
            }

            if(origin is PrimaryRulePart)
            {
                return DebugHelperForRuleInstance.ToString(origin as PrimaryRulePart);
            }

            return DebugHelperForRuleInstance.ToString(origin as SecondaryRulePart);
        }

        public string GetSenderExpressionNodeHumanizeDbgString()
        {
            if (SenderExpressionNode == null)
            {
                return string.Empty;
            }

            var origin = SenderExpressionNode.Origin;

            if (origin == null)
            {
                return string.Empty;
            }

            return DebugHelperForRuleInstance.ToString(origin);
        }
#endif
    }
}
