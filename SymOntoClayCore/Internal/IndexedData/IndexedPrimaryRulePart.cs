using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedPrimaryRulePart: IndexedBaseRulePart
    {
        public PrimaryRulePart OriginPrimaryRulePart { get; set; }

        /// <inheritdoc/>
        public override BaseRulePart OriginRulePart => OriginPrimaryRulePart;

        public List<IndexedSecondaryRulePart> SecondaryParts { get; set; } = new List<IndexedSecondaryRulePart>();

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public string GetHumanizeDbgString()
        {
            if(OriginPrimaryRulePart == null)
            {
                return string.Empty;
            }

            return DebugHelperForRuleInstance.ToString(OriginPrimaryRulePart);
        }

        public void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            options.Logger.Log($"Begin~~~~~~ GetHumanizeDbgString() = {GetHumanizeDbgString()}");
#endif

            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;

            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForExpression.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            queryExecutingCardForExpression.SenderIndexedRulePart = this;
            Expression.FillExecutingCard(queryExecutingCardForExpression, options);

#if DEBUG
            options.Logger.Log($"#$%^$%^^ queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

            foreach (var resultOfQueryToRelation in queryExecutingCardForExpression.ResultsOfQueryToRelationList)
            {
                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
            }

#if DEBUG
            options.Logger.Log("End");
#endif
        }
    }
}
