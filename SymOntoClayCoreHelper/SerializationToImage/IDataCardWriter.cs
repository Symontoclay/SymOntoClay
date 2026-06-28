using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardWriter: IDisposable
    {
        string BasePath { get; }
        void Write(IDataCard dataCard);
    }
}
