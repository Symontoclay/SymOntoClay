using System;
using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardReader : IDisposable
    {
        List<IDataCard> ReadAll();
    }
}
