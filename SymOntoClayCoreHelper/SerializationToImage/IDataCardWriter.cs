using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardWriter: IDisposable
    {
        void Write(IDataCard dataCard);
    }
}
