using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class RelationIndexedLogicalQueryNode: BaseIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.Relation;

        public ulong Key { get; set; }
        public int CountParams { get; set; }
        public bool IsQuestion { get; set; }
        public IList<BaseIndexedLogicalQueryNode> Params { get; set; }
        public IList<QueryExecutingCardAboutVar> VarsInfoList { get; set; }
        public IList<QueryExecutingCardAboutKnownInfo> KnownInfoList { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.PrintObjListProp(n, nameof(Params), Params);
            sb.PrintObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.PrintShortObjListProp(n, nameof(Params), Params);
            sb.PrintShortObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintShortObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.PrintBriefObjListProp(n, nameof(Params), Params);
            sb.PrintBriefObjListProp(n, nameof(VarsInfoList), VarsInfoList);
            sb.PrintBriefObjListProp(n, nameof(KnownInfoList), KnownInfoList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            options.Logger.Log($"IsQuestion = {IsQuestion}");
#endif

            if (IsQuestion)
            {
                FillExecutingCardForQuestion(queryExecutingCard, options);
                return;
            }

            throw new NotImplementedException();
        }

        private void FillExecutingCardForQuestion(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            options.Logger.Log($"Key = {Key}");
            options.Logger.Log($"IsQuestion = {IsQuestion}");
            options.Logger.Log($"Params.Count = {Params.Count}");
            foreach (var param in Params)
            {
                options.Logger.Log($"param = {param}");
            }
            options.Logger.Log($"VarsInfoList.Count = {VarsInfoList.Count}");
            foreach (var varInfo in VarsInfoList)
            {
                options.Logger.Log($"varInfo = {varInfo}");
            }
            options.Logger.Log($"queryExecutingCard = {queryExecutingCard}");
            options.Logger.Log($"queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString() = {queryExecutingCard.GetSenderExpressionNodeHumanizeDbgString()}");
            options.Logger.Log($"queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRulePartHumanizeDbgString()}");
            options.Logger.Log($"queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString() = {queryExecutingCard.GetSenderIndexedRuleInstanceHumanizeDbgString()}");
            options.Logger.Log($"GetHumanizeDbgString() = {GetHumanizeDbgString()}");
#endif



            throw new NotImplementedException();
        }
    }
}
