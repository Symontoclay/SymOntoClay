using Newtonsoft.Json;
using NLog;
using SymOntoClay.Serialization.Implementation.InternalPlainObjects;
using SymOntoClay.Serialization.Settings;
using SymOntoClay.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SymOntoClay.Serialization.Implementation
{
    public class Deserializer : IDeserializer
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public Deserializer(IDeserializationContext deserializationContext)
        {
            _deserializationContext = deserializationContext;
        }

        private readonly IDeserializationContext _deserializationContext;

        /// <inheritdoc/>
        public T Deserialize<T>()
        {
            var rootFileFullName = Path.Combine(_deserializationContext.RootDirName, "root.json");

#if DEBUG
            _logger.Info($"rootFileFullName = {rootFileFullName}");
#endif

            var fileContent = File.ReadAllText(rootFileFullName);

#if DEBUG
            _logger.Info($"fileContent = {fileContent}");
#endif

            var rootObject = JsonConvert.DeserializeObject<RootObject>(fileContent);

#if DEBUG
            _logger.Info($"rootObject = {rootObject}");
#endif

            return (T)NDeserialize(rootObject.Data);
        }

        /// <inheritdoc/>
        public T GetDeserializedObject<T>(ObjectPtr objectPtr)
        {
#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            return (T)GetDeserializedObject(objectPtr);
        }

        private object GetDeserializedObject(ObjectPtr objectPtr)
        {
#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            if (objectPtr.IsNull)
            {
                return null;
            }

            var instanceId = objectPtr.Id;

            if (_deserializationContext.TryGetDeserializedObject(instanceId, out var instance))
            {
                return instance;
            }

            return NDeserialize(objectPtr);
        }

        private object NDeserialize(ObjectPtr objectPtr)
        {
#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            var fileName = $"{objectPtr.Id}.json";

            var fullFileName = Path.Combine(_deserializationContext.HeapDirName, fileName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
            _logger.Info($"objectPtr.TypeName = {objectPtr.TypeName}");
#endif

            //var type = Type.GetType(objectPtr.TypeName, false, true);

            var type = GetType(objectPtr.TypeName);

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
#endif

            if(type.FullName.StartsWith("System.Action"))
            {
                return NDeserializeAction(type, objectPtr, fullFileName);
            }

            if (type.FullName.StartsWith("System.Func"))
            {
                return NDeserializeAction(type, objectPtr, fullFileName);
            }

            switch (type.FullName)
            {
                case "System.Object":
                    return NDeserializeBareObject(objectPtr);

                case "System.Threading.CancellationTokenSource":
                    return NDeserializeCancellationTokenSource(objectPtr, fullFileName);

                case "System.Threading.CancellationTokenSource+Linked1CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+Linked2CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+LinkedNCancellationTokenSource":
                    return NDeserializeLinkedCancellationTokenSource(objectPtr, fullFileName);

                case "System.Threading.CancellationToken":
                    return NDeserializeCancellationToken(objectPtr, fullFileName);

                case "SymOntoClay.Threading.CustomThreadPool":
                    return NDeserializeCustomThreadPool(objectPtr, fullFileName);
            }

            switch (type.Name)
            {
                case "List`1":
                    return NDeserializeGenericList(type, objectPtr, fullFileName);

                case "Dictionary`2":
                    return NDeserializeGenericDictionary(type, objectPtr, fullFileName);

                default:
                    if (type.FullName.StartsWith("System.Threading.") ||
                        type.FullName.StartsWith("System.Collections."))
                    {
                        throw new NotImplementedException("CE6ABC55-44C9-49EF-B431-738880E68CB4");
                    }

                    return NDeserializeComposite(type, objectPtr, fullFileName);
            }
        }

        private Type GetType(string typeName)
        {
#if DEBUG
            _logger.Info($"typeName = {typeName}");
#endif

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                //_logger.Info($"assembly.FullName = {assembly.FullName}");

                var type = assembly.GetType(typeName);

#if DEBUG
                //_logger.Info($"type?.FullName = {type?.FullName}");
#endif

                if (type == null)
                {
                    continue;
                }

                return type;
            }

            return null;
        }

        private Type GetActionFactoryType(string id)
        {
#if DEBUG
            _logger.Info($"id = {id}");
#endif

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                //_logger.Info($"assembly.FullName = {assembly.FullName}");

                var types = assembly.GetTypes().Where(p => p.GetInterfaces().Any(x => x == typeof(ISocSerializableActionFactory)));

                _logger.Info($"types.Count() = {types.Count()}");

                if (types.Count() == 0)
                {
                    continue;
                }

                foreach (var type in types)
                {
                    _logger.Info($"type.FullName = {type.FullName}");

                    var atributes = type.CustomAttributes;

                    _logger.Info($"atributes.Count() = {atributes.Count()}");

                    foreach (var attribute in atributes)
                    {
                        _logger.Info($"attribute.AttributeType.FullName = {attribute.AttributeType.FullName}");

                        _logger.Info($"attribute.ConstructorArguments.Count() = {attribute.ConstructorArguments.Count()}");

                        foreach (var argument in attribute.ConstructorArguments)
                        {
                            _logger.Info($"argument.Value = {argument.Value}");
                            _logger.Info($"argument.ArgumentType.FullName = {argument.ArgumentType.FullName}");
                        }

                        _logger.Info($"attribute.NamedArguments.Count() = {attribute.NamedArguments.Count()}");

                        foreach (var argument in attribute.NamedArguments)
                        {
                            _logger.Info($"argument.MemberName = {argument.MemberName}");
                            _logger.Info($"argument.TypedValue = {argument.TypedValue}");
                        }
                    }
                }

                //var targetType = assembly.GetTypes().Where(p => p.GetInterfaces().Any(x => x == typeof(ISocSerializableActionFactory)))
                //    .FirstOrDefault(p => p.CustomAttributes.Any(x => x.ConstructorArguments.Any(y => y.ArgumentType == typeof(string) && (string)y.Value == id)));

                //_logger.Info($"targetType?.FullName = {targetType?.FullName}");

                //if(targetType != null)
                //{
                //    return targetType;
                //}

                throw new NotImplementedException("93F4E4E5-41DE-4600-BC21-AF0C92E31FB4");
            }

            return null;
        }

        private bool IsSerializable(Type type)
        {
#if DEBUG
            _logger.Info($"type.GetInterfaces() = {JsonConvert.SerializeObject(type.GetInterfaces().Select(p => p.FullName), Formatting.Indented)}");
#endif

            return type.GetInterfaces().Any(p => p == typeof(ISerializable));
        }

        private object NDeserializeAction(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            throw new NotImplementedException("76BF75F1-51C6-49F0-9E08-55B45771828F");
        }

        private object NDeserializeComposite(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var fields = SerializationHelper.GetFields(instance);

#if DEBUG
            _logger.Info($"fields.Length = {fields.Length}");
#endif

            var plainObjectsDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(fullFileName), SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"plainObjectsDict = {JsonConvert.SerializeObject(plainObjectsDict, Formatting.Indented)}");
            _logger.Info($"plainObjectsDict = {JsonConvert.SerializeObject(plainObjectsDict, SerializationHelper.JsonSerializerSettings)}");
#endif

            foreach (var field in fields)
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
                _logger.Info($"field.FieldType?.FullName = {field.FieldType?.FullName}");
#endif

                var plainValue = plainObjectsDict[field.Name];

#if DEBUG
                _logger.Info($"plainValue = {plainValue}");
#endif

                var itemValue = ConvertObjectCollectionValueFromSerializableFormat(plainValue);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
                _logger.Info($"itemValue?.GetType()?.FullName = {itemValue?.GetType()?.FullName}");
                _logger.Info($"itemValue?.GetType()?.Name = {itemValue?.GetType()?.Name}");
                _logger.Info($"field.FieldType.FullName = {field.FieldType.FullName}");
                _logger.Info($"field.FieldType.Name = {field.FieldType.Name}");
#endif

                field.SetValue(instance, ChangeType(itemValue, field.FieldType));
            }

            return instance;
        }

        private object ChangeType(object value, Type conversionType)
        {
            if(conversionType.Name == "Nullable`1" && value.GetType().Name != "Nullable`1")
            {
                var realFieldType = conversionType.GenericTypeArguments[0];

#if DEBUG
                _logger.Info($"realFieldType.FullName = {realFieldType.FullName}");
#endif

                return Convert.ChangeType(value, realFieldType);
            }

            return Convert.ChangeType(value, conversionType);
        }

        private object NDeserializeCustomThreadPool(ObjectPtr objectPtr, string fullFileName)
        {
            var plainObject = JsonConvert.DeserializeObject<CustomThreadPoolPo>(File.ReadAllText(fullFileName), SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            var settings = GetDeserializedObject<CustomThreadPoolSerializationSettings>(plainObject.Settings);

#if DEBUG
            _logger.Info($"settings = {JsonConvert.SerializeObject(settings, Formatting.Indented)}");
#endif

            var instance = new CustomThreadPool(settings.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                settings.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                settings.CancellationToken ?? CancellationToken.None);

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            return instance;
        }

        private object NDeserializeCancellationToken(ObjectPtr objectPtr, string fullFileName)
        {
            var plainObject = JsonConvert.DeserializeObject<CancellationTokenPo>(File.ReadAllText(fullFileName), SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            var cancelationTokenSource = GetDeserializedObject(plainObject.Source) as CancellationTokenSource;

#if DEBUG
            _logger.Info($"cancelationTokenSource = {JsonConvert.SerializeObject(cancelationTokenSource, Formatting.Indented)}");
#endif

            if (cancelationTokenSource == null)
            {
                return CancellationToken.None;
            }

            return cancelationTokenSource.Token;
        }

        private object NDeserializeLinkedCancellationTokenSource(ObjectPtr objectPtr, string fullFileName)
        {
            var plainObject = JsonConvert.DeserializeObject<LinkedCancellationTokenSourcePo>(File.ReadAllText(fullFileName), SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            var settings = GetDeserializedObject<LinkedCancellationTokenSourceSerializationSettings>(plainObject.Settings);

#if DEBUG
            _logger.Info($"settings = {JsonConvert.SerializeObject(settings, Formatting.Indented)}");
#endif

            var instance = CreateLinkedTokenSource(settings);

            if (plainObject.IsCancelled)
            {
                instance.Cancel();
            }

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            return instance;
        }

        private CancellationTokenSource CreateLinkedTokenSource(LinkedCancellationTokenSourceSerializationSettings settings)
        {
            var token1 = settings.Token1;
            var token2 = settings.Token2;
            var token3 = settings.Token3;
            var token4 = settings.Token4;
            var token5 = settings.Token5;
            var token6 = settings.Token6;
            var token7 = settings.Token7;
            var token8 = settings.Token8;
            var token9 = settings.Token9;
            var token10 = settings.Token10;

            if (token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue && token5.HasValue &&
                token6.HasValue && token7.HasValue && token8.HasValue && token9.HasValue && token10.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value, token4.Value, token5.Value,
                    token6.Value, token7.Value, token8.Value, token9.Value, token10.Value);
            }

            if (token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue && token5.HasValue &&
                token6.HasValue && token7.HasValue && token8.HasValue && token9.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value, token4.Value, token5.Value,
                    token6.Value, token7.Value, token8.Value, token9.Value);
            }

            if (token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue &&
                token5.HasValue && token6.HasValue && token7.HasValue && token8.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value, token4.Value,
                    token5.Value, token6.Value, token7.Value, token8.Value);
            }

            if (token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue &&
                token5.HasValue && token6.HasValue && token7.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value, token4.Value,
                    token5.Value, token6.Value, token7.Value);
            }

            if (token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue && token5.HasValue && token6.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value, token4.Value, token5.Value, token6.Value);
            }

            if (token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue && token5.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value, token4.Value, token5.Value);
            }

            if (token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value, token4.Value);
            }

            if (token1.HasValue && token2.HasValue && token3.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value, token3.Value);
            }

            if (token1.HasValue && token2.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value, token2.Value);
            }

            if (token1.HasValue)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(token1.Value);
            }

            throw new NotSupportedException("2A54A7AB-56FC-4EA0-BA51-E33D70841177");
        }

        private object NDeserializeCancellationTokenSource(ObjectPtr objectPtr, string fullFileName)
        {
            var plainObject = JsonConvert.DeserializeObject<CancellationTokenSourcePo>(File.ReadAllText(fullFileName), SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            var instance = new CancellationTokenSource();

            if (plainObject.IsCancelled)
            {
                instance.Cancel();
            }

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            return instance;
        }

        private object NDeserializeBareObject(ObjectPtr objectPtr)
        {
            var instance = new object();

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            return instance;
        }

        private object NDeserializeGenericDictionary(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            if (type.IsGenericType)
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
                        return NDeserializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(type, objectPtr, fullFileName);
                    }
                    else
                    {
                        if (SerializationHelper.IsObject(valueGenericParameterType))
                        {
                            return NDeserializeGenericDictionaryWithPrimitiveKeyAndObjectValue(type, keyGenericParameterType, objectPtr, fullFileName);
                        }
                        else
                        {
                            return NDeserializeGenericDictionaryWithPrimitiveKeyAndCompositeValue(type, keyGenericParameterType, objectPtr, fullFileName);
                        }
                    }
                }
                else
                {
                    if (SerializationHelper.IsObject(keyGenericParameterType))
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            return NDeserializeGenericDictionaryWithObjectKeyAndPrimitiveValue(type, valueGenericParameterType, objectPtr, fullFileName);
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                return NDeserializeGenericDictionaryWithObjectKeyAndObjectValue(type, objectPtr, fullFileName);
                            }
                            else
                            {
                                return NDeserializeGenericDictionaryWithObjectKeyAndCompositeValue(type, objectPtr, fullFileName);
                            }
                        }
                    }
                    else
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            return NDeserializeGenericDictionaryWithCompositeKeyAndPrimitiveValue(type, valueGenericParameterType, objectPtr, fullFileName);
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                return NDeserializeGenericDictionaryWithCompositeKeyAndObjectValue(type, objectPtr, fullFileName);
                            }
                            else
                            {
                                return NDeserializeGenericDictionaryWithCompositeKeyAndCompositeValue(type, objectPtr, fullFileName);
                            }
                        }
                    }
                }
            }

            throw new NotImplementedException("5BA58F8C-E959-486F-87D7-3211CDC20B8B");
        }

        private object NDeserializeGenericDictionaryWithCompositeKeyAndCompositeValue(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(ObjectPtr), typeof(ObjectPtr));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

#if DEBUG
            _logger.Info($"File.ReadAllText(fullFileName) = '{File.ReadAllText(fullFileName)}'");
#endif

            var listWithPlainObjects = (IList)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), listWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            var keyProperty = keyValuePairType.GetProperty("Key");
            var valueProperty = keyValuePairType.GetProperty("Value");

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {plainObjectItem}");
                _logger.Info($"plainObjectItem?.GetType()?.FullName = {plainObjectItem?.GetType()?.FullName}");
#endif

                var plainObjectItemKey = keyProperty.GetValue(plainObjectItem);
                var plainObjectItemValue = valueProperty.GetValue(plainObjectItem);

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemKey = GetDeserializedObject((ObjectPtr)plainObjectItemKey);

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
#endif

                var itemValue = GetDeserializedObject((ObjectPtr)plainObjectItemValue);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
#endif

                dictionary.Add(itemKey, itemValue);
            }

            return instance;
        }

        private object NDeserializeGenericDictionaryWithCompositeKeyAndObjectValue(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(ObjectPtr), typeof(object));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

#if DEBUG
            _logger.Info($"File.ReadAllText(fullFileName) = '{File.ReadAllText(fullFileName)}'");
#endif

            var listWithPlainObjects = (IList)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), listWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            var keyProperty = keyValuePairType.GetProperty("Key");
            var valueProperty = keyValuePairType.GetProperty("Value");

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {plainObjectItem}");
                _logger.Info($"plainObjectItem?.GetType()?.FullName = {plainObjectItem?.GetType()?.FullName}");
#endif

                var plainObjectItemKey = keyProperty.GetValue(plainObjectItem);
                var plainObjectItemValue = valueProperty.GetValue(plainObjectItem);

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemKey = GetDeserializedObject((ObjectPtr)plainObjectItemKey);

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
#endif

                var itemValue = ConvertObjectCollectionValueFromSerializableFormat(plainObjectItemValue);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
#endif

                dictionary.Add(itemKey, itemValue);
            }

            return instance;
        }

        private object NDeserializeGenericDictionaryWithCompositeKeyAndPrimitiveValue(Type type, Type valueGenericParameterType, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(ObjectPtr), valueGenericParameterType);

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

#if DEBUG
            _logger.Info($"File.ReadAllText(fullFileName) = '{File.ReadAllText(fullFileName)}'");
#endif

            var listWithPlainObjects = (IList)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), listWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            var keyProperty = keyValuePairType.GetProperty("Key");
            var valueProperty = keyValuePairType.GetProperty("Value");

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {plainObjectItem}");
                _logger.Info($"plainObjectItem?.GetType()?.FullName = {plainObjectItem?.GetType()?.FullName}");
#endif

                var plainObjectItemKey = keyProperty.GetValue(plainObjectItem);
                var plainObjectItemValue = valueProperty.GetValue(plainObjectItem);

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemKey = GetDeserializedObject((ObjectPtr)plainObjectItemKey);

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
#endif

                dictionary.Add(itemKey, plainObjectItemValue);
            }

            return instance;
        }

        private object NDeserializeGenericDictionaryWithObjectKeyAndCompositeValue(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(object), typeof(ObjectPtr));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

#if DEBUG
            _logger.Info($"File.ReadAllText(fullFileName) = '{File.ReadAllText(fullFileName)}'");
#endif

            var listWithPlainObjects = (IList)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), listWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            var keyProperty = keyValuePairType.GetProperty("Key");
            var valueProperty = keyValuePairType.GetProperty("Value");

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {plainObjectItem}");
                _logger.Info($"plainObjectItem?.GetType()?.FullName = {plainObjectItem?.GetType()?.FullName}");
#endif

                var plainObjectItemKey = keyProperty.GetValue(plainObjectItem);
                var plainObjectItemValue = valueProperty.GetValue(plainObjectItem);

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemKey = ConvertObjectCollectionValueFromSerializableFormat(plainObjectItemKey);

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
#endif

                var itemValue = GetDeserializedObject((ObjectPtr)plainObjectItemValue);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
#endif

                dictionary.Add(itemKey, itemValue);
            }

            return instance;
        }
        
        private object NDeserializeGenericDictionaryWithObjectKeyAndObjectValue(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(object), typeof(object));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

#if DEBUG
            _logger.Info($"File.ReadAllText(fullFileName) = '{File.ReadAllText(fullFileName)}'");
#endif

            var listWithPlainObjects = (IList)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), listWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            var keyProperty = keyValuePairType.GetProperty("Key");
            var valueProperty = keyValuePairType.GetProperty("Value");

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {plainObjectItem}");
                _logger.Info($"plainObjectItem?.GetType()?.FullName = {plainObjectItem?.GetType()?.FullName}");
#endif

                var plainObjectItemKey = keyProperty.GetValue(plainObjectItem);
                var plainObjectItemValue = valueProperty.GetValue(plainObjectItem);

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemKey = ConvertObjectCollectionValueFromSerializableFormat(plainObjectItemKey);

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
#endif

                var itemValue = ConvertObjectCollectionValueFromSerializableFormat(plainObjectItemValue);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
#endif

                dictionary.Add(itemKey, itemValue);
            }

            return instance;
        }

        private object NDeserializeGenericDictionaryWithObjectKeyAndPrimitiveValue(Type type, Type valueGenericParameterType, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(object), valueGenericParameterType);

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

#if DEBUG
            _logger.Info($"File.ReadAllText(fullFileName) = '{File.ReadAllText(fullFileName)}'");
#endif

            var listWithPlainObjects = (IList)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), listWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            var keyProperty = keyValuePairType.GetProperty("Key");
            var valueProperty = keyValuePairType.GetProperty("Value");

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {plainObjectItem}");
                _logger.Info($"plainObjectItem?.GetType()?.FullName = {plainObjectItem?.GetType()?.FullName}");
#endif

                var plainObjectItemKey = keyProperty.GetValue(plainObjectItem);
                var plainObjectItemValue = valueProperty.GetValue(plainObjectItem);

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemKey = ConvertObjectCollectionValueFromSerializableFormat(plainObjectItemKey);

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
#endif

                dictionary.Add(itemKey, plainObjectItemValue);
            }

            return instance;
        }

        private object NDeserializeGenericDictionaryWithPrimitiveKeyAndCompositeValue(Type type, Type keyGenericParameterType, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var dictWithPlainObjectsType = typeof(Dictionary<,>).MakeGenericType(keyGenericParameterType, typeof(ObjectPtr));

            var dictWithPlainObjects = (IDictionary)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), dictWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"dictWithPlainObjects = {JsonConvert.SerializeObject(dictWithPlainObjects, Formatting.Indented)}");
#endif

            foreach (DictionaryEntry plainObjectItem in dictWithPlainObjects)
            {
                var plainObjectItemKey = plainObjectItem.Key;
                var plainObjectItemValue = plainObjectItem.Value;

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemValue = GetDeserializedObject((ObjectPtr)plainObjectItemValue);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
#endif

                dictionary.Add(plainObjectItemKey, itemValue);
            }

            return instance;
        }

        private object NDeserializeGenericDictionaryWithPrimitiveKeyAndObjectValue(Type type, Type keyGenericParameterType, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var dictionary = (IDictionary)instance;

            var dictWithPlainObjectsType = typeof(Dictionary<,>).MakeGenericType(keyGenericParameterType, typeof(object));

            var dictWithPlainObjects = (IDictionary)JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), dictWithPlainObjectsType, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"dictWithPlainObjects = {JsonConvert.SerializeObject(dictWithPlainObjects, Formatting.Indented)}");
#endif

            foreach (DictionaryEntry plainObjectItem in dictWithPlainObjects)
            {
                var plainObjectItemKey = plainObjectItem.Key;
                var plainObjectItemValue = plainObjectItem.Value;

#if DEBUG
                _logger.Info($"plainObjectItemKey = {plainObjectItemKey}");
                _logger.Info($"plainObjectItemKey?.GetType()?.FullName = {plainObjectItemKey?.GetType()?.FullName}");
                _logger.Info($"plainObjectItemValue = {plainObjectItemValue}");
#endif

                var itemValue = ConvertObjectCollectionValueFromSerializableFormat(plainObjectItemValue);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
#endif

                dictionary.Add(plainObjectItemKey, itemValue);
            }

            return instance;
        }

        private object NDeserializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), type, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"instance = {JsonConvert.SerializeObject(instance, Formatting.Indented)}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            return instance;
        }

        private object NDeserializeGenericList(Type type, ObjectPtr objectPtr, string fullFileName)
        {
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
                    return NDeserializeListWithPrimitiveParameter(type, objectPtr, fullFileName);                    
                }

                if (SerializationHelper.IsObject(genericParameterType))
                {
                    return NDeserializeListWithObjectParameter(type, objectPtr, fullFileName);
                }
            }

            return NDeserializeListWithCompositeParameter(type, objectPtr, fullFileName);
        }

        private object NDeserializeListWithCompositeParameter(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var list = (IList)instance;

            var listWithPlainObjects = JsonConvert.DeserializeObject<List<ObjectPtr>>(File.ReadAllText(fullFileName), SerializationHelper.JsonSerializerSettings);

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {JsonConvert.SerializeObject(plainObjectItem, Formatting.Indented)}");
#endif

#if DEBUG
                var itemType = plainObjectItem.GetType();
                _logger.Info($"itemType.FullName = {itemType.FullName}");
                _logger.Info($"itemType.Name = {itemType.Name}");
                _logger.Info($"itemType.IsGenericType = {itemType.IsGenericType}");
#endif

                list.Add(GetDeserializedObject((ObjectPtr)plainObjectItem));
            }

#if DEBUG
            _logger.Info($"list = {JsonConvert.SerializeObject(list, Formatting.Indented)}");
#endif

            return instance;
        }

        private object NDeserializeListWithObjectParameter(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var list = (List<object>)instance;

            var listWithPlainObjects = JsonConvert.DeserializeObject<List<object>>(File.ReadAllText(fullFileName), SerializationHelper.JsonSerializerSettings);

            foreach (var plainObjectItem in listWithPlainObjects)
            {
#if DEBUG
                _logger.Info($"plainObjectItem = {JsonConvert.SerializeObject(plainObjectItem, Formatting.Indented)}");
#endif

                list.Add(ConvertObjectCollectionValueFromSerializableFormat(plainObjectItem));
            }

#if DEBUG
            _logger.Info($"list = {JsonConvert.SerializeObject(list, Formatting.Indented)}");
#endif

            return list;
        }

        private object NDeserializeListWithPrimitiveParameter(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), type, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"instance = {JsonConvert.SerializeObject(instance, Formatting.Indented)}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            return instance;
        }

        private object ConvertObjectCollectionValueFromSerializableFormat(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (SerializationHelper.IsPrimitiveType(value))
            {
                return value;
            }

            return GetDeserializedObject((ObjectPtr)value);
        }

        private object NDeserializeISerializable(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            throw new NotImplementedException("9B62CD6D-EE3F-4E60-8F87-784BA0C1CAF1");

//            var instance = Activator.CreateInstance(type);

//#if DEBUG
//            _logger.Info($"instance = {instance}");
//#endif

//            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

//            var serializable = (ISerializable)instance;

//            var plainObject = JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), serializable.GetPlainObjectType(), SerializationHelper.JsonSerializerSettings);

//#if DEBUG
//            _logger.Info($"plainObject = {plainObject}");
//#endif

//            serializable.OnReadPlainObject(plainObject, this);

//#if DEBUG
//            _logger.Info($"serializable = {serializable}");
//#endif

//            return serializable;
        }

        /// <inheritdoc/>
        public T GetAction<T>(string id, int index)
        {
#if DEBUG
            _logger.Info($"id = {id}");
#endif

            var actionFactoryType = GetActionFactoryType(id);

#if DEBUG
            _logger.Info($"actionFactoryType.FullName = {actionFactoryType.FullName}");
#endif

            var factoryInstance = (ISocSerializableActionFactory)Activator.CreateInstance(actionFactoryType);

            return (T)factoryInstance.GetAction(index);
        }
    }
}
