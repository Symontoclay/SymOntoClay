using System;
using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedObjectsPool: ISerializedObjectsPool
    {
        private Dictionary<object, SerializedValue> _serializedObjects = new Dictionary<object, SerializedValue>();

        /// <inheritdoc/>
        public bool IsSerialized(object obj)
        {
            return _serializedObjects.ContainsKey(obj);
        }

        /// <inheritdoc/>
        public bool TryGetSerializedValue(object obj, out SerializedValue serializedValue)
        {
            return _serializedObjects.TryGetValue(obj, out serializedValue);
        }

        /// <inheritdoc/>
        public SerializedValue RegSerializedValue(object obj)
        {
            throw new NotImplementedException("C18F7466-0119-42E3-9D52-8743C6579B15");
        }
    }
}
