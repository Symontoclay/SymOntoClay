using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardWriter: IDisposable
    {
        string RelativePath { get; }
        void Write(IDataCard dataCard);
    }
}
