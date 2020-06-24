using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IOperatorsStorage
    {
        KindOfStorage Kind { get; }

        IndexedOperator GetOperator(KindOfOperator kindOfOperator);
    }
}
