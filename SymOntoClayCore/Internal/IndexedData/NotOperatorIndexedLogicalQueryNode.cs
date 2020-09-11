using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class NotOperatorIndexedLogicalQueryNode: UnaryOperatorIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.UnaryOperator;

        /// <inheritdoc/>
        public override KindOfOperatorOfLogicalQueryNode KindOfOperator => KindOfOperatorOfLogicalQueryNode.Not;

        /// <inheritdoc/>
        public override void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            throw new NotImplementedException();
        }
    }
}
