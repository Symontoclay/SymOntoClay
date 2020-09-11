using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class IndexedRelationLogicalQueryNode: BaseIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.Relation;

        public ulong Key { get; set; }
        public int CountParams { get; set; }
        public bool IsQuestion { get; set; }
        public IList<BaseIndexedLogicalQueryNode> Params { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Key)} = {Key}");
            sb.AppendLine($"{spaces}{nameof(CountParams)} = {CountParams}");
            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.PrintObjListProp(n, nameof(Params), Params);

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

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            throw new NotImplementedException();
        }
    }
}
