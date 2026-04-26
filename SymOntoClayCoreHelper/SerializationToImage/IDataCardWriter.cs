using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardWriter: IDisposable
    {
        void Write(KindOfDataCard kindOfDataCard, object dataCard);
    }
}
