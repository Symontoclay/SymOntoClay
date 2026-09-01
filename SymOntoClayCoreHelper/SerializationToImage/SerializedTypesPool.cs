using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.CoreHelper.SerializerAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private Dictionary<int, string> _typeNamesDict = new Dictionary<int, string>();

        /// <inheritdoc/>
        public int NullTypeId => _nullTypeId;

        /// <inheritdoc/>
        public int GetOrRegisterType(Type type)
        {
#if DEBUG
            //_logger.Info($"type?.Name = {type?.Name}");
            //_logger.Info($"type?.FullName = {type?.FullName}");
#endif

            if(type == null)
            {
                return _nullTypeId;
            }

            var typeFullName = type.FullName;

            if (_typeIdsDict.TryGetValue(typeFullName, out var typeId))
            {
                return typeId;
            }

            typeId = GetTypeId();

            _typeIdsDict[typeFullName] = typeId;
            _typeNamesDict[typeId] = typeFullName;

            return typeId;
        }

        private int GetTypeId()
        {
            lock (_lock) 
            {
                _currentTypeId++;
                return _currentTypeId;
            }
        }

        /// <inheritdoc/>
        public Type GetTypeValue(int typeId)
        {
            lock (_lock)
            {
#if DEBUG
                _logger.Info($"typeId = {typeId}");
#endif

                if(typeId == _nullTypeId)
                {
                    return null;
                }

                var typeFullName = _typeNamesDict[typeId];

#if DEBUG
                _logger.Info($"typeFullName = {typeFullName}");
#endif

                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType(typeFullName, false, true))
                    .FirstOrDefault(t => t != null);

#if DEBUG
                _logger.Info($"type?.Name = {type?.Name}");
                _logger.Info($"type?.FullName = {type?.FullName}");
#endif

                return type;
            }
        }

        /// <inheritdoc/>
        public void Save(BinaryWriter writer)
        {
            var bsonSerializerAdapter = new BsonSerializerAdapter();
            var serializer = new JsonSerializer();

            writer.Write(_currentTypeId);

            var typeIdsDictData = bsonSerializerAdapter.Serialize(_typeIdsDict, serializer);

#if DEBUG
            //_logger.Info($"typeIdsDictData.Length = {typeIdsDictData.Length}");
#endif

            writer.Write(typeIdsDictData.Length);
            writer.Write(typeIdsDictData);

            var typeNamesDictData = bsonSerializerAdapter.Serialize(_typeNamesDict, serializer);

#if DEBUG
            //_logger.Info($"typeNamesDictData.Length = {typeNamesDictData.Length}");
#endif

            writer.Write(typeNamesDictData.Length);
            writer.Write(typeNamesDictData);
        }

        /// <inheritdoc/>
        public void Load(BinaryReader reader)
        {
            var bsonSerializerAdapter = new BsonSerializerAdapter();
            var serializer = new JsonSerializer();

            _currentTypeId = reader.ReadInt32();

#if DEBUG
            //_logger.Info($"_currentTypeId = {_currentTypeId}");
#endif

            var typeIdsDictDataLength = reader.ReadInt32();

#if DEBUG
            //_logger.Info($"typeIdsDictDataLength = {typeIdsDictDataLength}");
#endif

            var typeIdsDictData = reader.ReadBytes(typeIdsDictDataLength);

            _typeIdsDict = bsonSerializerAdapter.Deserialize<Dictionary<string, int>>(typeIdsDictData, serializer);

            var typeNamesDictDataLength = reader.ReadInt32();

            var typeNamesDictData = reader.ReadBytes(typeNamesDictDataLength);

            _typeNamesDict = bsonSerializerAdapter.Deserialize<Dictionary<int, string>>(typeNamesDictData, serializer);
        }
    }
}
