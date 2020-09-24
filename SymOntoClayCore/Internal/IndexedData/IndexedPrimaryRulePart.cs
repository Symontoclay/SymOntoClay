using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedPrimaryRulePart: IndexedBaseRulePart
    {
        public PrimaryRulePart OriginPrimaryRulePart { get; set; }

        /// <inheritdoc/>
        public override BaseRulePart OriginRulePart => OriginPrimaryRulePart;

        public IList<IndexedSecondaryRulePart> SecondaryParts { get; set; } = new List<IndexedSecondaryRulePart>();

        /// <inheritdoc/>
        public override IList<IndexedBaseRulePart> GetNextPartsList()
        {
            return SecondaryParts.Select(p => (IndexedBaseRulePart)p).ToList();
        }

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

        public void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            options.Logger.Log($"Begin~~~~~~ GetHumanizeDbgString() = {GetHumanizeDbgString()}");
#endif

#if DEBUG
            var senderIndexedRuleInstance = queryExecutingCard.SenderIndexedRuleInstance;
#endif
            var queryExecutingCardForExpression = new QueryExecutingCardForIndexedPersistLogicalData();

#if DEBUG
            queryExecutingCardForExpression.SenderIndexedRuleInstance = senderIndexedRuleInstance;
            queryExecutingCardForExpression.SenderIndexedRulePart = this;
#endif
            Expression.FillExecutingCard(queryExecutingCardForExpression, dataSource, options);

#if DEBUG
            options.Logger.Log($"#$%^$%^^ queryExecutingCardForExpression = {queryExecutingCardForExpression}");
#endif

            queryExecutingCard.IsSuccess = queryExecutingCardForExpression.IsSuccess;

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
