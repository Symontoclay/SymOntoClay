using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IChannelHandler
    {
        IndexedValue Read();
        IndexedValue Write(IndexedValue value);
    }
}
