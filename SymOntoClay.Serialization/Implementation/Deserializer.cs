using Newtonsoft.Json;
using NLog;
using SymOntoClay.Serialization.Implementation.InternalPlainObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
                    
                case "System.Threading.CancellationToken":
                    throw new NotImplementedException("2F82C7B6-68B0-4CCF-9358-AF662C2EA47E");
            }

            switch (type.Name)
            {
                case "List`1":
                    return NDeserializeGenericList(type, objectPtr, fullFileName);

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

        private object NDeserializeGenericList(Type type, ObjectPtr objectPtr, string fullFileName)
        {
            var genericParameterType = type.GetGenericArguments()[0];

#if DEBUG
            _logger.Info($"genericParameterType.FullName = {genericParameterType.FullName}");
            _logger.Info($"genericParameterType.Name = {genericParameterType.Name}");
            _logger.Info($"genericParameterType.IsGenericType = {genericParameterType.IsGenericType}");
            _logger.Info($"genericParameterType.IsPrimitive = {genericParameterType.IsPrimitive}");
#endif

            if (SerializationHelper.IsObject(genericParameterType))
            {
                return NDeserializeListWithObjectParameter(type, objectPtr, fullFileName);
            }

            if (SerializationHelper.IsPrimitiveType(genericParameterType))
            {
                throw new NotImplementedException("CCBC128D-B66C-4CC1-89E6-020262E7B424");
            }

            if (genericParameterType == typeof(ISerializable))
            {
                throw new NotImplementedException("A8778677-669D-4372-A3D6-E5404B46A447");
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
