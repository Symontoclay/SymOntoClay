using System;
using System.Collections.Generic;
using System.Globalization;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedObjectsPool: ISerializedObjectsPool
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public SerializedObjectsPool(ISerializedTypesPool serializedTypesPool, ITypesHelper typesHelper) 
        {
            _serializedTypesPool = serializedTypesPool;
            _typesHelper = typesHelper;
            _nullValue = new SerializedValue(KindOfSerializedValue.Null, _nullId, serializedTypesPool.NullTypeId);
        }

        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly ITypesHelper _typesHelper;
        private readonly int _nullId = 0;
        private readonly SerializedValue _nullValue;

        private Dictionary<object, SerializedValue> _serializedObjects = new Dictionary<object, SerializedValue>();
        private Dictionary<SerializedValue, object> _backSserializedObjectsDict = new Dictionary<SerializedValue, object>();

        private readonly object _lock = new object();
        private int _currentId = 0;

        /// <inheritdoc/>
        public bool IsSerialized(object obj, bool ignorePreregistered)
        {
            if(obj == null)
            {
                return true;
            }

            if(ignorePreregistered)
            {
                if (_serializedObjects.TryGetValue(obj, out var serializedValue))
                {
                    if(serializedValue.KindOfSerializedValue == KindOfSerializedValue.Preregistered)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
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
        public bool TryGetObject(SerializedValue serializedValue, out object obj)
        {
            if(serializedValue == null)
            {
                obj = null;
                return true;
            }

            if(serializedValue == _nullValue)
            {
                obj = null;
                return true;
            }

            return _backSserializedObjectsDict.TryGetValue(serializedValue, out obj);
        }

        /// <inheritdoc/>
        public SerializedValue RegSerializedValue(object obj, SerializedObjectsPoolMode mode)
        {
            if (obj == null)
            {
                return _nullValue;
            }

            if (_serializedObjects.TryGetValue(obj, out var serializedValue))
            {
                return serializedValue;
            }

            var type = obj.GetType();

            var id = GetId();
            var typeId = _serializedTypesPool.GetOrRegisterType(type);
            var kindOfSerializedValue = _typesHelper.GetKindOfSerializedValue(type);

#if DEBUG
            //_logger.Info($"id = {id}");
            //_logger.Info($"typeId = {typeId}");
            //_logger.Info($"kindOfSerializedValue = {kindOfSerializedValue}");
#endif

            switch(mode)
            {
                case SerializedObjectsPoolMode.ExternalValue:
                    kindOfSerializedValue = KindOfSerializedValue.ExternalValue;
                    break;

                case SerializedObjectsPoolMode.IsPreregistered:
                    kindOfSerializedValue = KindOfSerializedValue.Preregistered;
                    break;
            }

            var literal = string.Empty;

            if(kindOfSerializedValue == KindOfSerializedValue.Literal)
            {
                literal = _typesHelper.ToString(obj);
            }

#if DEBUG
            //_logger.Info($"literal = {literal}");
#endif

            serializedValue = new SerializedValue(kindOfSerializedValue, id, typeId, literal);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            _serializedObjects[obj] = serializedValue;
            _backSserializedObjectsDict[serializedValue] = obj;

            return serializedValue;
        }

        /// <inheritdoc/>
        public SerializedValue GetOrRegSerializedValue(object obj, SerializedObjectsPoolMode mode)
        {
            if(TryGetSerializedValue(obj, out var serializedValue))
            {
                return serializedValue;
            }

            return RegSerializedValue(obj, mode);
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
