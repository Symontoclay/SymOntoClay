using System;
using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedObjectsPool: ISerializedObjectsPool
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public SerializedObjectsPool(ISerializedTypesPool serializedTypesPool) 
        {
            _serializedTypesPool = serializedTypesPool;
            _nullValue = new SerializedValue(KindOfSerializedValue.Null, _nullId, serializedTypesPool.NullTypeId);
        }

        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly int _nullId = 0;
        private readonly SerializedValue _nullValue;

        private Dictionary<object, SerializedValue> _serializedObjects = new Dictionary<object, SerializedValue>();

        private readonly object _lock = new object();
        private int _currentId = 0;

        /// <inheritdoc/>
        public bool IsSerialized(object obj)
        {
            if(obj == null)
            {
                return true;
            }

            return _serializedObjects.ContainsKey(obj);
        }

        /// <inheritdoc/>
        public bool TryGetSerializedValue(object obj, out SerializedValue serializedValue)
        {
            if (obj == null)
            {
                serializedValue = _nullValue;
                return true;
            }

            return _serializedObjects.TryGetValue(obj, out serializedValue);
        }

        /// <inheritdoc/>
        public SerializedValue RegSerializedValue(object obj)
        {
            if (obj == null)
            {
                return _nullValue;
            }

            if (_serializedObjects.TryGetValue(obj, out var serializedValue))
            {
                return serializedValue;
            }

            var id = GetId();
            var typeId = _serializedTypesPool.GetOrRegisterType(obj.GetType());

#if DEBUG
            _logger.Info($"id = {id}");
            _logger.Info($"typeId = {typeId}");
#endif

            throw new NotImplementedException("C18F7466-0119-42E3-9D52-8743C6579B15");
        }

        private int GetId()
        {
            lock (_lock)
            {
                _currentId++;
                return _currentId;
            }
        }
    }
}
