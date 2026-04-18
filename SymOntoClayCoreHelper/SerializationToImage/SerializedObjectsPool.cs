using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedObjectsPool: ISerializedObjectsPool
    {
        /// <inheritdoc/>
        public bool IsSerialized(object obj)
        {
            throw new NotImplementedException("C8DD4FD5-C8A9-406A-9387-FBDFD0B5782E");
        }

        /// <inheritdoc/>
        public bool TryGetSerializedValue(object obj, out SerializedValue serializedValue)
        {
            throw new NotImplementedException("C9B63C8E-1FA0-4F8D-A9A3-6B181700B088");
        }

        /// <inheritdoc/>
        public SerializedValue RegSerializedValue(object obj)
        {
            throw new NotImplementedException("C18F7466-0119-42E3-9D52-8743C6579B15");
        }
    }
}
