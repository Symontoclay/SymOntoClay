using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SymOntoClay.Serialization.Implementation.InternalPlainObjects;
using SymOntoClay.Serialization.Settings;
using SymOntoClay.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

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

            var fullFileName = Path.Combine(_serializationContext.RootDirName, fileName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            File.WriteAllText(fullFileName, JsonConvert.SerializeObject(rootObject));
        }

        /// <inheritdoc/>
        public ObjectPtr GetSerializedObjectPtr(object obj)
        {
            return GetSerializedObjectPtr(obj, null);
        }

        /// <inheritdoc/>
        public ObjectPtr GetSerializedObjectPtr(object obj, object settingsParameter)
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

                switch (type.FullName)
                {
                    case "System.Object":
                        return NSerializeBareObject(obj);

                    case "System.Threading.CancellationTokenSource":
                        return NSerializeCancellationTokenSource((CancellationTokenSource)obj);

                    case "System.Threading.CancellationTokenSource+Linked1CancellationTokenSource":
                    case "System.Threading.CancellationTokenSource+Linked2CancellationTokenSource":
                    case "System.Threading.CancellationTokenSource+LinkedNCancellationTokenSource":
                        return NSerializeLinkedCancellationTokenSource((CancellationTokenSource)obj, settingsParameter as LinkedCancellationTokenSourceSerializationSettings);
                    
                    case "System.Threading.CancellationToken":
                        return NSerializeCancellationToken((CancellationToken)obj);

                    case "SymOntoClay.Threading.CustomThreadPool":
                        return NSerializeCustomThreadPool((CustomThreadPool)obj, settingsParameter as CustomThreadPoolSerializationSettings);
                }

                switch (type.Name)
                {
                    case "List`1":
                        return NSerializeGenericList((IEnumerable)obj);

                    case "Dictionary`2":
                        return NSerializeGenericDictionary((IDictionary)obj);

                    default:
                        throw new NotImplementedException("B3784FDD-7AFC-4947-AA62-00398400E52B");
                }
            }
            else
            {
                return NSerialize(serializable);
            }
        }

        private ObjectPtr NSerializeCustomThreadPool(CustomThreadPool customThreadPool, CustomThreadPoolSerializationSettings settingsParameter)
        {
#if DEBUG
            _logger.Info($"settingsParameter = {settingsParameter}");
#endif

            if(settingsParameter == null)
            {
                throw new ArgumentNullException(nameof(settingsParameter), $"Serialization parameter is required for type {nameof(CustomThreadPool)}.");
            }

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, customThreadPool.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(customThreadPool, objectPtr);

            var plainObject = new CustomThreadPoolPo();
            plainObject.Settings = GetSerializedObjectPtr(settingsParameter);

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            WriteToFile(plainObject, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
#if DEBUG
            _logger.Info($"cancellationTokenSource.IsCancellationRequested = {cancellationTokenSource.IsCancellationRequested}");
#endif

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, cancellationTokenSource.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(cancellationTokenSource, objectPtr);

            var plainObject = new CancellationTokenSourcePo();
            plainObject.IsCancelled = cancellationTokenSource.IsCancellationRequested;

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject)}");
#endif

            WriteToFile(plainObject, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeLinkedCancellationTokenSource(CancellationTokenSource cancellationTokenSource, LinkedCancellationTokenSourceSerializationSettings settingsParameter)
        {
#if DEBUG
            _logger.Info($"settingsParameter = {settingsParameter}");
            _logger.Info($"cancellationTokenSource.IsCancellationRequested = {cancellationTokenSource.IsCancellationRequested}");
#endif

            if (settingsParameter == null)
            {
                throw new ArgumentNullException(nameof(settingsParameter), $"Serialization parameter is required for linked {nameof(CancellationTokenSource)}.");
            }

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, cancellationTokenSource.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(cancellationTokenSource, objectPtr);

            var plainObject = new LinkedCancellationTokenSourcePo();
            plainObject.IsCancelled = cancellationTokenSource.IsCancellationRequested;
            plainObject.Settings = GetSerializedObjectPtr(settingsParameter);

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            WriteToFile(plainObject, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeCancellationToken(CancellationToken cancellationToken)
        {
            var sourceField = cancellationToken.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.DeclaredOnly).Single(p => p.Name == "_source");

            var fieldValue = sourceField.GetValue(cancellationToken);

#if DEBUG
            _logger.Info($"fieldValue?.GetType() = {fieldValue?.GetType()}");
            if (fieldValue != null)
            {
                _logger.Info($"((CancellationTokenSource)fieldValue).IsCancellationRequested = {((CancellationTokenSource)fieldValue).IsCancellationRequested}");
            }
#endif

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, cancellationToken.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            var plainObject = new CancellationTokenPo();
            plainObject.Source = GetSerializedObjectPtr(fieldValue);

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            WriteToFile(plainObject, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeBareObject(object obj)
        {
            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, obj.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(obj, objectPtr);

            var plainObject = new object();

#if DEBUG
            _logger.Info($"plainObject = {plainObject}");
#endif

            WriteToFile(plainObject, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeGenericDictionary(IDictionary dictionary)
        {
            var type = dictionary.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if(type.IsGenericType)
            {
                var keyGenericParameterType = type.GetGenericArguments()[0];

#if DEBUG
                _logger.Info($"keyGenericParameterType.FullName = {keyGenericParameterType.FullName}");
                _logger.Info($"keyGenericParameterType.Name = {keyGenericParameterType.Name}");
                _logger.Info($"keyGenericParameterType.IsGenericType = {keyGenericParameterType.IsGenericType}");
                _logger.Info($"keyGenericParameterType.IsPrimitive = {keyGenericParameterType.IsPrimitive}");
#endif

                var valueGenericParameterType = type.GetGenericArguments()[1];

#if DEBUG
                _logger.Info($"valueGenericParameterType.FullName = {valueGenericParameterType.FullName}");
                _logger.Info($"valueGenericParameterType.Name = {valueGenericParameterType.Name}");
                _logger.Info($"valueGenericParameterType.IsGenericType = {valueGenericParameterType.IsGenericType}");
                _logger.Info($"valueGenericParameterType.IsPrimitive = {valueGenericParameterType.IsPrimitive}");
#endif

                if (SerializationHelper.IsPrimitiveType(keyGenericParameterType))
                {
                    if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                    {
                        return NSerializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(dictionary, keyGenericParameterType, valueGenericParameterType);
                    }
                    else
                    {
                        if(SerializationHelper.IsObject(valueGenericParameterType))
                        {
                            throw new NotImplementedException("CF8D5901-5409-4347-8064-D555A0B7A25F");
                        }
                        else
                        {
                            throw new NotImplementedException("EF93BCF0-BAD9-44A8-9D16-A4581F6988ED");
                        }                        
                    }
                }
                else
                {
                    if(SerializationHelper.IsObject(keyGenericParameterType))
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            throw new NotImplementedException("ED7A7AC5-8982-47EE-99BC-DA5EAE30C0D1");
                        }
                        else
                        {
                            throw new NotImplementedException("3F2EA372-5B3D-4FC5-A3E3-0AE8721FB406");
                        }
                    }
                    else
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            throw new NotImplementedException("358AA9A3-7827-4B1A-847A-3CC44F48F5A2");
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                throw new NotImplementedException("FABC69AF-4885-4F02-A0CA-5B1E9BE1F2D3");
                            }
                            else
                            {
                                throw new NotImplementedException("31F1F400-81C7-4110-8D7F-D25F4818F413");
                            }
                        }
                    }
                }
            }

            throw new NotImplementedException("D55AE149-D344-4855-8EC0-2AD18C0F90D5");
        }

        private ObjectPtr NSerializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(IDictionary dictionary, Type keyGenericParameterType, Type valueGenericParameterType)
        {
            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(dictionary, objectPtr);

#if DEBUG
            _logger.Info($"dictionary = {JsonConvert.SerializeObject(dictionary, Formatting.Indented)}");
#endif

            WriteToFile(dictionary, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeGenericList(IEnumerable enumerable)
        {
            var type = enumerable.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if(type.IsGenericType)
            {
                var genericParameterType = type.GetGenericArguments()[0];

#if DEBUG
                _logger.Info($"genericParameterType.FullName = {genericParameterType.FullName}");
                _logger.Info($"genericParameterType.Name = {genericParameterType.Name}");
                _logger.Info($"genericParameterType.IsGenericType = {genericParameterType.IsGenericType}");
                _logger.Info($"genericParameterType.IsPrimitive = {genericParameterType.IsPrimitive}");
#endif

                if (SerializationHelper.IsPrimitiveType(genericParameterType))
                {
                    throw new NotImplementedException("9B48CB69-4F44-4C9E-9C7D-B57593946B70");
                }

                if (SerializationHelper.IsObject(genericParameterType))
                {
                    return NSerializeListWithObjectParameter(enumerable);
                }
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

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, enumerable.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(enumerable, objectPtr);

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

                    throw new NotImplementedException("1A7ACA8A-95A7-4096-94E6-53BF131E8960");
                }
                else
                {
                    listWithPlainObjects.Add(NSerialize(serializable));
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
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

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, enumerable.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            _serializationContext.RegObjectPtr(enumerable, objectPtr);

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

                    throw new NotImplementedException("79D76024-320E-4BA4-AB03-DA46959D2894");
                }
                else
                {
                    listWithPlainObjects.Add(NSerialize(serializable));
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
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

            var fullFileName = Path.Combine(_serializationContext.HeapDirName, fileName);

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
