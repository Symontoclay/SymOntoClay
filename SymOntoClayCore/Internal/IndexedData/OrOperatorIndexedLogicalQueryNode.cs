﻿using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData
{
    public class OrOperatorIndexedLogicalQueryNode: BinaryOperatorIndexedLogicalQueryNode
    {
        /// <inheritdoc/>
        public override KindOfLogicalQueryNode Kind => KindOfLogicalQueryNode.BinaryOperator;

        /// <inheritdoc/>
        public override KindOfOperatorOfLogicalQueryNode KindOfOperator => KindOfOperatorOfLogicalQueryNode.Or;

        /// <inheritdoc/>
        public override void FillExecutingCard(QueryExecutingCardForIndexedPersistLogicalData queryExecutingCard, OptionsOfFillExecutingCard options)
        {
            throw new NotImplementedException();
        }
    }
}
