using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public interface IDataCardDictionary : IDisposable
    {
        IDataCardWithHeader GetDataCardByHeader(SerializedValue header);
    }
}
