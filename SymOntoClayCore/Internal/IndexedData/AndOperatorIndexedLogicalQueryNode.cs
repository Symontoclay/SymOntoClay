using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class AndOperatorIndexedLogicalQueryNode: BinaryOperatorIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.BinaryOperator;

        /// <inheritdoc/>
        public override KindOfOperatorOfLogicalQueryNode KindOfOperator => KindOfOperatorOfLogicalQueryNode.And;

        /// <inheritdoc/>
        public override void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, ConsolidatedDataSource dataSource, OptionsOfFillExecutingCard options)
        {
            throw new NotImplementedException();
        }
    }
}
