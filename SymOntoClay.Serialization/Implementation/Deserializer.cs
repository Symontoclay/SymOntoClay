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

            switch(type.FullName)
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
                    {
                        if (IsSerializable(type))
                        {
                            return NDeserializeISerializable(type, objectPtr, fullFileName);
                        }
                    }
                    throw new NotImplementedException("615629EF-E2D6-4457-8445-55EA563ECF49");
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

                if(type == null)
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

            if(cancelationTokenSource == null)
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

            if(token1.HasValue && token2.HasValue && token3.HasValue && token4.HasValue && token5.HasValue &&
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
            
            if(plainObject.IsCancelled)
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
                            throw new NotImplementedException("0F2D8965-160F-4A1E-B8C8-2D8E5A7D2817");
                        }
                        else
                        {
                            throw new NotImplementedException("0B528C10-9041-4738-87B3-FB3B63788460");
                        }
                    }
                }
                else
                {
                    if (SerializationHelper.IsObject(keyGenericParameterType))
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            throw new NotImplementedException("B97EE398-6CA5-49EA-B035-5FBB495C5E9C");
                        }
                        else
                        {
                            throw new NotImplementedException("BD4D4EDA-EAAF-4D79-BCDA-97A975C59842");
                        }
                    }
                    else
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            throw new NotImplementedException("C1FE08FC-CFDE-4C47-BD33-190B2E673E88");
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                throw new NotImplementedException("7608FB03-EF92-4EC5-A127-AA7A9DEC9DB1");
                            }
                            else
                            {
                                throw new NotImplementedException("7B46F83C-7895-410A-B886-04210DDBB662");
                            }
                        }
                    }
                }
            }

            throw new NotImplementedException("5BA58F8C-E959-486F-87D7-3211CDC20B8B");
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
                    throw new NotImplementedException("CCBC128D-B66C-4CC1-89E6-020262E7B424");
                }

                if (SerializationHelper.IsObject(genericParameterType))
                {
                    return NDeserializeListWithObjectParameter(type, objectPtr, fullFileName);
                }
            }

            return NDeserializeListWithPossibleSerializebleParameter(type, objectPtr, fullFileName);
        }

        private object NDeserializeListWithPossibleSerializebleParameter(Type type, ObjectPtr objectPtr, string fullFileName)
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

                var itemType = plainObjectItem.GetType();

#if DEBUG
                _logger.Info($"itemType.FullName = {itemType.FullName}");
                _logger.Info($"itemType.Name = {itemType.Name}");
                _logger.Info($"itemType.IsGenericType = {itemType.IsGenericType}");
#endif

                if (SerializationHelper.IsObjectPtr(itemType))
                {
                    list.Add(GetDeserializedObject((ObjectPtr)plainObjectItem));

                    continue;
                }

                throw new NotImplementedException("EFAED86D-2860-4B64-801C-3615B9F7D7E7");
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

                if (SerializationHelper.IsPrimitiveType(plainObjectItem))
                {
                    list.Add(plainObjectItem);

                    continue;
                }

                var itemType = plainObjectItem.GetType();

#if DEBUG
                _logger.Info($"itemType.FullName = {itemType.FullName}");
                _logger.Info($"itemType.Name = {itemType.Name}");
                _logger.Info($"itemType.IsGenericType = {itemType.IsGenericType}");
#endif

                if (SerializationHelper.IsObjectPtr(itemType))
                {
                    list.Add(GetDeserializedObject((ObjectPtr)plainObjectItem));

                    continue;
                }

                throw new NotImplementedException("1AF9ED38-8427-42DF-A6E6-C2009A57DE30");

                //if (IsSerializable(type))
                //{
                //    return NDeserializeISerializable<T>(, objectPtr, fullFileName);
                //}

                //var serializable = plainObjectItem as ISerializable;

                //if (serializable == null)
                //{

                //    throw new NotImplementedException("FA3ADB12-D19E-4191-A1F6-34A0B1EB5A3D");
                //}
                //else
                //{
                //    list.Add(NDeserializeISerializable<T>(serializable));
                //}
            }

#if DEBUG
            _logger.Info($"list = {JsonConvert.SerializeObject(list, Formatting.Indented)}");
#endif

            return list;
        }

        private object NDeserializeISerializable(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var instance = Activator.CreateInstance(type);

#if DEBUG
            _logger.Info($"instance = {instance}");
#endif

            _deserializationContext.RegDeserializedObject(objectPtr.Id, instance);

            var serializable = (ISerializable)instance;

            var plainObject = JsonConvert.DeserializeObject(File.ReadAllText(fullFileName), serializable.GetPlainObjectType(), SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"plainObject = {plainObject}");
#endif

            serializable.OnReadPlainObject(plainObject, this);

#if DEBUG
            _logger.Info($"serializable = {serializable}");
#endif

            return serializable;
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
