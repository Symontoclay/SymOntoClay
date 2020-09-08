using SymOntoClay.Core.Internal.CodeModel;
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
            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, OptionsOfFillExecutingCard options)
        {
            options.Logger.Log("Begin");

            var queryExecutingCardForPart_1 = new QueryExecutingCardForIndexedPersistLogicalData();
            queryExecutingCardForPart_1.SenderIndexedRuleInstance = this;

            PrimaryPart.FillExecutingCard(queryExecutingCardForPart_1, options);

            foreach (var resultOfQueryToRelation in queryExecutingCardForPart_1.ResultsOfQueryToRelationList)
            {
                queryExecutingCard.ResultsOfQueryToRelationList.Add(resultOfQueryToRelation);
            }

            options.Logger.Log("End");
        }
    }
}
