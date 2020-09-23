using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IVarStorage: ISpecificStorage
    {
        void SetSystemValue(IndexedStrongIdentifierValue varName, IndexedValue value);
        IndexedValue GetSystemValueDirectly(IndexedStrongIdentifierValue varName);
        void SetValue(IndexedStrongIdentifierValue varName, IndexedValue value);
        IndexedValue GetValueDirectly(IndexedStrongIdentifierValue varName);
    }
}
