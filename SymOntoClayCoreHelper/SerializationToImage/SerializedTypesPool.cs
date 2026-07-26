using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.CoreHelper.SerializerAdapters;
using System;
using System.Collections.Generic;
using System.IO;

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
                return 0;
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

            throw new NotImplementedException("C9AB9158-B8A1-4EF5-BE96-7863DF398EE4");
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

            throw new NotImplementedException("C651F307-54AA-4A2F-9F53-19CB61449F1D");
        }
    }
}
