using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class OperatorsResolver: BaseResolver
    {
        public OperatorsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedOperator GetOperator(KindOfOperator kindOfOperator, IStorage storage)
        {
#if DEBUG
            Log($"kindOfOperator = {kindOfOperator}");
#endif

            throw new NotImplementedException();
        }
    }
}
