using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedRuleInstance: IndexedAnnotatedItem
    {
        public RuleInstance Origin { get; set; }
        public IndexedStrongIdentifierValue Name { get; set; }

        public KindOfRuleInstance Kind { get; set; } = KindOfRuleInstance.Undefined;

        public ulong Key { get; set; }

        public bool IsRule { get; set; }
        public IndexedPrimaryRulePart PrimaryPart { get; set; }
        public List<IndexedSecondaryRulePart> SecondaryParts { get; set; } = new List<IndexedSecondaryRulePart>();
        public IList<ulong> KeysOfPrimaryRecords { get; set; } = new List<ulong>();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode() ^ PrimaryPart.GetLongHashCode();

            if(!SecondaryParts.IsNullOrEmpty())
            {
                foreach(var secondaryPart in SecondaryParts)
                {
                    result ^= secondaryPart.GetLongHashCode();
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Origin), Origin);
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(IsRule)} = {IsRule}");

            sb.PrintObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Origin), Origin);
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(IsRule)} = {IsRule}");

            sb.PrintShortObjProp(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintShortObjListProp(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(Origin), Origin);
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(IsRule)} = {IsRule}");

            sb.PrintExisting(n, nameof(PrimaryPart), PrimaryPart);
            sb.PrintExistingList(n, nameof(SecondaryParts), SecondaryParts);
            sb.PrintPODList(n, nameof(KeysOfPrimaryRecords), KeysOfPrimaryRecords);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            return $"{spaces}{DebugHelperForRuleInstance.ToString(Origin)}";
        }

        public void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
#if DEBUG
            //options.Logger.Log("Begin");
#endif

            var queryExecutingCardForPart_1 = new QueryExecutingCardForIndexedPersistLogicalData();

#if DEBUG
            queryExecutingCardForPart_1.SenderIndexedRuleInstance = this;
#endif

            PrimaryPart.FillExecutingCard(queryExecutingCardForPart_1, dataSource, options);

            queryExecutingCard.IsSuccess = queryExecutingCardForPart_1.IsSuccess;

            foreach (var resultOfQueryToRelation in queryExecutingCardForPart_1.ResultsOfQueryToRelationList)
            {
                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
            }
#if DEBUG
            //options.Logger.Log("End");
#endif
        }
    }
}
