using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardWriter
    {
        void Write(KindOfDataCard kindOfDataCard, object dataCard);
    }
}
