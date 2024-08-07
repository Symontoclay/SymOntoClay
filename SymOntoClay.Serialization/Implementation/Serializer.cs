using Newtonsoft.Json;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.Serialization.Implementation
{
    public class Serializer : ISerializer
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif
        public Serializer(ISerializationContext serializationContext)
        {
            _serializationContext = serializationContext;
        }

        private readonly ISerializationContext _serializationContext;

        /// <inheritdoc/>
        public void Serialize(ISerializable serializable)
        {
#if DEBUG
            _logger.Info($"serializable = {serializable}");
#endif

            if (_serializationContext.IsSerialized(serializable))
            {
                return;
            }

            var rootObject = new RootObject();
            rootObject.Data = NSerialize(serializable);

#if DEBUG
            _logger.Info($"rootObject = {rootObject}");
#endif

            var fileName = $"root.json";

            var fullFileName = Path.Combine(_serializationContext.DirName, fileName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            File.WriteAllText(fullFileName, JsonConvert.SerializeObject(rootObject));
        }

        /// <inheritdoc/>
        public ObjectPtr GetSerializedObjectPtr(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            if (obj == null)
            {
                return new ObjectPtr(isNull: true);
            }

            if (_serializationContext.TryGetObjectPtr(obj, out var objectPtr))
            {
                return objectPtr;
            }

            var serializable = obj as ISerializable;

            if (serializable == null)
            {
                var type = obj.GetType();

#if DEBUG
                _logger.Info($"type.FullName = {type.FullName}");
                _logger.Info($"type.Name = {type.Name}");
                _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

                switch (type.Name)
                {
                    case "List`1":
                        return NSerializeGenericList((IEnumerable)obj);

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return NSerialize(serializable);
            }
        }

        private ObjectPtr NSerializeGenericList(IEnumerable enumerable)
        {
            var type = enumerable.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            var genericParameterType = type.GetGenericArguments()[0];

#if DEBUG
            _logger.Info($"genericParameterType.FullName = {genericParameterType.FullName}");
            _logger.Info($"genericParameterType.Name = {genericParameterType.Name}");
            _logger.Info($"genericParameterType.IsGenericType = {genericParameterType.IsGenericType}");
            _logger.Info($"genericParameterType.IsPrimitive = {genericParameterType.IsPrimitive}");
#endif

            if (SerializationHelper.IsObject(genericParameterType))
            {
                return NSerializeListWithObjectParameter(enumerable);
            }

            if(SerializationHelper.IsPrimitiveType(genericParameterType))
            {
                throw new NotImplementedException();
            }

            if(genericParameterType == typeof(ISerializable))
            {
                throw new NotImplementedException();
            }

            return NSerializeListWithPossibleSerializebleParameter(enumerable);
        }

        private ObjectPtr NSerializeListWithPossibleSerializebleParameter(IEnumerable enumerable)
        {
#if DEBUG
            var type = enumerable.GetType();
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            var listWithPlainObjects = new List<ObjectPtr>();

            foreach (var item in enumerable)
            {
                var serializable = item as ISerializable;

                if (serializable == null)
                {
                    var itemType = item.GetType();

#if DEBUG
                    _logger.Info($"itemType.FullName = {itemType.FullName}");
                    _logger.Info($"itemType.Name = {itemType.Name}");
                    _logger.Info($"itemType.IsGenericType = {itemType.IsGenericType}");
#endif

                    throw new NotImplementedException();
                }
                else
                {
                    listWithPlainObjects.Add(NSerialize(serializable));
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, enumerable.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            WriteToFile(listWithPlainObjects, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeListWithObjectParameter(IEnumerable enumerable)
        {
#if DEBUG
            var type = enumerable.GetType();
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            var listWithPlainObjects = new List<object>();

            foreach (var item in enumerable)
            {
                if (SerializationHelper.IsPrimitiveType(item))
                {
                    listWithPlainObjects.Add(item);

                    continue;
                }

                var serializable = item as ISerializable;

                if (serializable == null)
                {
                    var itemType = item.GetType();

#if DEBUG
                    _logger.Info($"itemType.FullName = {itemType.FullName}");
                    _logger.Info($"itemType.Name = {itemType.Name}");
                    _logger.Info($"itemType.IsGenericType = {itemType.IsGenericType}");
#endif

                    throw new NotImplementedException();
                }
                else
                {
                    listWithPlainObjects.Add(NSerialize(serializable));
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, enumerable.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            WriteToFile(listWithPlainObjects, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerialize(ISerializable serializable)
        {
            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, serializable.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(serializable, objectPtr);

            var plainObject = Activator.CreateInstance(serializable.GetPlainObjectType());

            serializable.OnWritePlainObject(plainObject, this);

#if DEBUG
            _logger.Info($"plainObject = {plainObject}");
#endif

            WriteToFile(plainObject, instanceId);

            return objectPtr;
        }

        private void WriteToFile(object value, string instanceId)
        {
            var fileName = $"{instanceId}.json";

            var fullFileName = Path.Combine(_serializationContext.DirName, fileName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            File.WriteAllText(fullFileName, JsonConvert.SerializeObject(value, SerializationHelper.JsonSerializerSettings));
        }

        private string CreateInstanceId()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}
