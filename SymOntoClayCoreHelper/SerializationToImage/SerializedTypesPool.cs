using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
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
            writer.Write(_currentTypeId);

            using var ms = new MemoryStream();
            using var bsonWriter = new BsonDataWriter(ms);

            var serializer = new JsonSerializer();
            serializer.Serialize(bsonWriter, _typeIdsDict);

            var typeIdsDictData = ms.ToArray();

#if DEBUG
            _logger.Info($"typeIdsDictData.Length = {typeIdsDictData.Length}");
#endif

            writer.Write(typeIdsDictData.Length);
            writer.Write(typeIdsDictData);
        }

        /// <inheritdoc/>
        public void LoadFromStream(Stream stream)
        {
            throw new NotImplementedException("C2D406C9-A03D-4025-B29F-AFAE64054AAB");
        }
    }
}
