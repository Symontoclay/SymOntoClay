using System;
using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedTypesPool: ISerializedTypesPool
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly object _lock = new object();

        private int _nullTypeId = 0;
        private int _currentTypeId = 0;

        private Dictionary<string, int> _typeIdsDict = new Dictionary<string, int>();

        /// <inheritdoc/>
        public int NullTypeId => _nullTypeId;

        /// <inheritdoc/>
        public int GetOrRegisterType(Type type)
        {
#if DEBUG
            _logger.Info($"type?.Name = {type?.Name}");
            _logger.Info($"type?.FullName = {type?.FullName}");
#endif

            if(type == null)
            {
                return 0;
            }

            var typeFullName = type.FullName;

            if (_typeIdsDict.TryGetValue(typeFullName, out var typeId))
            {
                return typeId;
            }

            typeId = GetTypeId();

            _typeIdsDict[typeFullName] = typeId;

            return typeId;
        }

        /// <inheritdoc/>
        public KindOfSerializedValue GetKindOfSerializedValue(Type type)
        {
            if(type == null)
            {
                return KindOfSerializedValue.Null;
            }

            throw new NotImplementedException("C2172792-58FF-4E95-BA4A-7092F929938E");
        }

        private int GetTypeId()
        {
            lock (_lock) 
            {
                _currentTypeId++;
                return _currentTypeId;
            }
        }
    }
}
