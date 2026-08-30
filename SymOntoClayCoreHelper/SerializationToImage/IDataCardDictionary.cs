using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardDictionary : IDisposable
    {
        bool TryGetDataCardByHeader(SerializedValue header, out IDataCardWithHeader card);
    }
}
