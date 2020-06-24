using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.OperatorsStorage
{
    public class OperatorsStorage : BaseLoggedComponent, IOperatorsStorage
    {
        public OperatorsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        public IndexedOperator GetOperator(KindOfOperator kindOfOperator)
        {
#if DEBUG
            Log($"kindOfOperator = {kindOfOperator}");
#endif

            throw new NotImplementedException();
        }
    }
}
