﻿using Newtonsoft.Json;
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

                if (SerializationHelper.IsPrimitiveType(type))
                {
                    return BoxPrimitiveType(obj);
                }

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

        private ObjectPtr BoxPrimitiveType(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var type = obj.GetType();

            var objectPtr = ObjectPtr.FromPrimitive(instanceId, type.FullName, SerializationHelper.PrimitiveTypeToString(type, obj));

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            return objectPtr;
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
                        return NSerializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(dictionary);
                    }
                    else
                    {
                        if(SerializationHelper.IsObject(valueGenericParameterType))
                        {
                            return NSerializeGenericDictionaryWithPrimitiveKeyAndObjectValue(dictionary, keyGenericParameterType);
                        }
                        else
                        {
                            return NSerializeGenericDictionaryWithPrimitiveKeyAndCompositeValue(dictionary, keyGenericParameterType);                            
                        }
                    }
                }
                else
                {
                    if(SerializationHelper.IsObject(keyGenericParameterType))
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            return NSerializeGenericDictionaryWithObjectKeyAndPrimitiveValue(dictionary, valueGenericParameterType);
                        }
                        else
                        {
                            if(SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                return NSerializeGenericDictionaryWithObjectKeyAndObjectValue(dictionary);
                            }
                            else
                            {
                                return NSerializeGenericDictionaryWithObjectKeyAndCompositeValue(dictionary);
                            }
                        }
                    }
                    else
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            return NSerializeGenericDictionaryWithCompositeKeyAndPrimitiveValue(dictionary);
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                return NSerializeGenericDictionaryWithCompositeKeyAndObjectValue(dictionary);
                            }
                            else
                            {
                                return NSerializeGenericDictionaryWithCompositeKeyAndCompositeValue(dictionary);
                            }
                        }
                    }
                }
            }

            throw new NotImplementedException("D55AE149-D344-4855-8EC0-2AD18C0F90D5");
        }

        private ObjectPtr NSerializeGenericDictionaryWithCompositeKeyAndCompositeValue(IDictionary dictionary)
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

            throw new NotImplementedException("31F1F400-81C7-4110-8D7F-D25F4818F413");
        }

        private ObjectPtr NSerializeGenericDictionaryWithCompositeKeyAndObjectValue(IDictionary dictionary)
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

            throw new NotImplementedException("FABC69AF-4885-4F02-A0CA-5B1E9BE1F2D3");
        }

        private ObjectPtr NSerializeGenericDictionaryWithCompositeKeyAndPrimitiveValue(IDictionary dictionary)
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

            throw new NotImplementedException("358AA9A3-7827-4B1A-847A-3CC44F48F5A2");
        }

        private ObjectPtr NSerializeGenericDictionaryWithObjectKeyAndCompositeValue(IDictionary dictionary)
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

            throw new NotImplementedException("11F3491A-91B7-4D38-B8AB-8C1EA3401D87");
        }

        private ObjectPtr NSerializeGenericDictionaryWithObjectKeyAndObjectValue(IDictionary dictionary)
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

            throw new NotImplementedException("3F2EA372-5B3D-4FC5-A3E3-0AE8721FB406");
        }

        private ObjectPtr NSerializeGenericDictionaryWithObjectKeyAndPrimitiveValue(IDictionary dictionary, Type valueGenericParameterType)
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

            var dictWithPlainObjectsType = typeof(Dictionary<,>).MakeGenericType(typeof(ObjectPtr), valueGenericParameterType);

            var dictWithPlainObjects = (IDictionary)Activator.CreateInstance(dictWithPlainObjectsType);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                var plainKey = GetSerializedObjectPtr(itemKey);

#if DEBUG
                _logger.Info($"plainKey = {plainKey}");
#endif

                dictWithPlainObjects[plainKey] = itemValue;
            }

#if DEBUG
            _logger.Info($"dictWithPlainObjects = {JsonConvert.SerializeObject(dictWithPlainObjects, Formatting.Indented)}");
#endif

            WriteToFile(dictWithPlainObjects, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeGenericDictionaryWithPrimitiveKeyAndCompositeValue(IDictionary dictionary, Type keyGenericParameterType)
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

            var dictWithPlainObjectsType = typeof(Dictionary<,>).MakeGenericType(keyGenericParameterType, typeof(ObjectPtr));

            var dictWithPlainObjects = (IDictionary)Activator.CreateInstance(dictWithPlainObjectsType);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                var plainValue = GetSerializedObjectPtr(itemValue);

#if DEBUG
                _logger.Info($"plainValue = {plainValue}");
#endif

                dictWithPlainObjects[itemKey] = plainValue;
            }

#if DEBUG
            _logger.Info($"dictWithPlainObjects = {JsonConvert.SerializeObject(dictWithPlainObjects, Formatting.Indented)}");
#endif

            WriteToFile(dictWithPlainObjects, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeGenericDictionaryWithPrimitiveKeyAndObjectValue(IDictionary dictionary, Type keyGenericParameterType)
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

            var dictWithPlainObjectsType = typeof(Dictionary<,>).MakeGenericType(keyGenericParameterType, typeof(ObjectPtr));

            var dictWithPlainObjects = (IDictionary)Activator.CreateInstance(dictWithPlainObjectsType);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                var plainValue = GetSerializedObjectPtr(itemValue);

#if DEBUG
                _logger.Info($"plainValue = {plainValue}");
#endif

                dictWithPlainObjects[itemKey] = plainValue;
            }

#if DEBUG
            _logger.Info($"dictWithPlainObjects = {JsonConvert.SerializeObject(dictWithPlainObjects, Formatting.Indented)}");
#endif

            WriteToFile(dictWithPlainObjects, instanceId);

            return objectPtr;
        }

        private ObjectPtr NSerializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(IDictionary dictionary)
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
                listWithPlainObjects.Add(GetSerializedObjectPtr(item));
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

                listWithPlainObjects.Add(GetSerializedObjectPtr(item));
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
