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
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
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
        public void Serialize(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            if (_serializationContext.IsSerialized(obj))
            {
                return;
            }

            var rootObject = new RootObject();
            rootObject.Data = GetSerializedObjectPtr(obj, null, string.Empty, KindOfSerialization.General, null, obj);

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

        private ObjectPtr SearchForMainFieldDeclaration(object targetObject, object rootObj)
        {
#if DEBUG
            _logger.Info($"targetObject = {targetObject}");
            _logger.Info($"rootObj = {rootObj}");
#endif

            throw new NotImplementedException("1142100F-33D7-4595-BA02-1D59FD3B7A65");
        }

        private ObjectPtr GetSerializedObjectPtr(object obj, object settingsParameter, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
            _logger.Info($"settingsParameter = {settingsParameter}");
            _logger.Info($"parentObjInfo = {parentObjInfo}");
            _logger.Info($"kindOfSerialization = {kindOfSerialization}");
            _logger.Info($"targetObject = {targetObject}");
            _logger.Info($"rootObj = {rootObj}");
#endif

            if (obj == null)
            {
                return new ObjectPtr(isNull: true);
            }

            if (_serializationContext.TryGetObjectPtr(obj, out var objectPtr))
            {
                return objectPtr;
            }

            var type = obj.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if(type.FullName.StartsWith("System.Action"))
            {
                return NSerializeAction(obj, settingsParameter as ActionPo, type, parentObjInfo, kindOfSerialization, targetObject, rootObj);
            }

            if (type.FullName.StartsWith("System.Func"))
            {
                return NSerializeAction(obj, settingsParameter as ActionPo, type, parentObjInfo, kindOfSerialization, targetObject, rootObj);
            }

            switch (type.FullName)
            {
                case "System.Object":
                    return NSerializeBareObject(obj, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                case "System.Threading.CancellationTokenSource":
                    return NSerializeCancellationTokenSource((CancellationTokenSource)obj, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                case "System.Threading.CancellationTokenSource+Linked1CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+Linked2CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+LinkedNCancellationTokenSource":
                    return NSerializeLinkedCancellationTokenSource((CancellationTokenSource)obj, settingsParameter as LinkedCancellationTokenSourceSerializationSettings,
                        parentObjInfo, kindOfSerialization, targetObject, rootObj);

                case "System.Threading.CancellationToken":
                    return NSerializeCancellationToken((CancellationToken)obj, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                case "SymOntoClay.Threading.CustomThreadPool":
                    return NSerializeCustomThreadPool((CustomThreadPool)obj, settingsParameter as CustomThreadPoolSerializationSettings, parentObjInfo,
                        kindOfSerialization, targetObject, rootObj);
            }

            switch (type.Name)
            {
                case "List`1":
                    return NSerializeGenericList((IEnumerable)obj, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                case "Dictionary`2":
                    return NSerializeGenericDictionary((IDictionary)obj, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                default:
                    if (type.FullName.StartsWith("System.Threading.") ||
                        type.FullName.StartsWith("System.Collections."))
                    {
                        throw new NotImplementedException("4161028A-A2DB-41F0-8D53-6BCC81D317A4");
                    }

                    return NSerializeComposite(obj, parentObjInfo, kindOfSerialization, targetObject, rootObj);
            }
        }

        private ObjectPtr NSerializeAction(object obj, ActionPo settingsParameter, Type type, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
#if DEBUG
            _logger.Info($"settingsParameter = {settingsParameter}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(obj, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            if (settingsParameter == null)
            {
                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var foundObject = SearchForMainFieldDeclaration(obj, rootObj);

                            if (foundObject == null)
                            {
                                var errorSb = new StringBuilder($"Serialization parameter is required for linked {nameof(CancellationTokenSource)}.");

                                errorSb.Append(parentObjInfo);

                                throw new ArgumentNullException(nameof(settingsParameter), errorSb.ToString());
                            }
                            else
                            {
                                return foundObject;
                            }
                        }
                        break;

                        d

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }

                throw new ArgumentNullException(nameof(settingsParameter), $"Serialization parameter is required for type {type.Name}.");
            }

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, type.FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(obj, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        _serializationContext.RegObjectPtr(obj, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

#if DEBUG
            _logger.Info($"settingsParameter = {JsonConvert.SerializeObject(settingsParameter, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(settingsParameter, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        WriteToFile(settingsParameter, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeComposite(object obj, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(obj, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var type = obj.GetType();

            var objectPtr = new ObjectPtr(instanceId, type.FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(obj, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        _serializationContext.RegObjectPtr(obj, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var plainObjectsDict = new Dictionary<string, object>();

            var properties = SerializationHelper.GetProperties(type);

#if DEBUG
            _logger.Info($"properties.Length = {properties.Length}");
#endif

            foreach (var property in properties)
            {
#if DEBUG
                _logger.Info($"property.Name = {property.Name}");
                _logger.Info($"property.CustomAttributes.Count() = {property.CustomAttributes.Count()}");
                foreach (var attr in property.CustomAttributes)
                {
                    _logger.Info($"attr.AttributeType?.FullName = {attr.AttributeType?.FullName}");

                    foreach (var ctorArg in attr.ConstructorArguments)
                    {
                        _logger.Info($"ctorArg.Value = {ctorArg.Value}");
                    }

                    foreach (var namedArg in attr.NamedArguments)
                    {
                        _logger.Info($"namedArg.MemberName = {namedArg.MemberName}");
                        _logger.Info($"namedArg.TypedValue.Value = {namedArg.TypedValue.Value}");
                    }
                }
#endif
            }

            var propertiesAttributesDict = properties.ToDictionary(p => p.Name, p => p.CustomAttributes);

            var fields = SerializationHelper.GetFields(type);

#if DEBUG
            _logger.Info($"fields.Length = {fields.Length}");
#endif

            foreach (var field in fields)
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
                _logger.Info($"field.FieldType.FullName = {field.FieldType.FullName}");
                _logger.Info($"field.FieldType.Name = {field.FieldType.Name}");
                _logger.Info($"field.CustomAttributes.Count() = {field.CustomAttributes.Count()}");
#endif

                foreach (var attr in field.CustomAttributes)
                {
#if DEBUG
                    _logger.Info($"attr.AttributeType?.FullName = {attr.AttributeType?.FullName}");
#endif

                    foreach (var ctorArg in attr.ConstructorArguments)
                    {
#if DEBUG
                        _logger.Info($"ctorArg.Value = {ctorArg.Value}");
#endif
                    }

                    foreach (var namedArg in attr.NamedArguments)
                    {
#if DEBUG
                        _logger.Info($"namedArg.MemberName = {namedArg.MemberName}");
                        _logger.Info($"namedArg.TypedValue.Value = {namedArg.TypedValue.Value}");
#endif
                    }
                }

                var customAttributes = field.CustomAttributes;

                var customAttributeDataFromProperties = GetCustomAttributeDataFromProperties(propertiesAttributesDict, field.Name);

#if DEBUG
                _logger.Info($"customAttributeDataFromProperties?.Count() = {customAttributeDataFromProperties?.Count()}");
#endif

                if(customAttributeDataFromProperties?.Any() ?? false)
                {
                    customAttributes = customAttributes.Concat(customAttributeDataFromProperties);
                }

                if(IsNoSerialiable(customAttributes))
                {
                    continue;
                }

                var itemValue = field.GetValue(obj);

#if DEBUG
                _logger.Info($"itemValue = {itemValue}");
#endif

                var settingsParameterName = GetSettingsParameterName(customAttributes);

#if DEBUG
                _logger.Info($"settingsParameterName = {settingsParameterName}");
#endif

                var settingsParameter = GetSettingsParameter(fields, obj, settingsParameterName);

#if DEBUG
                _logger.Info($"settingsParameter = {settingsParameter}");
#endif

                var actionPlainObject = CreateActionPlainObject(fields, obj, customAttributes);

#if DEBUG
                _logger.Info($"actionPlainObject = {actionPlainObject}");
#endif

                var fieldParentObjInfo = $"{parentObjInfo}.{type.FullName}.{field.Name}";

#if DEBUG
                _logger.Info($"fieldParentObjInfo = {fieldParentObjInfo}");
#endif

                var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, settingsParameter ?? actionPlainObject, fieldParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        plainObjectsDict[field.Name] = plainValueResult.ConvertedObject;
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(obj, targetObject))
                        {f
                            var foundObject = plainValueResult.FoundObject;

                            if (foundObject == null)
                            {
                                continue;
                            }

                            return foundObject;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"plainObjectsDict = {JsonConvert.SerializeObject(plainObjectsDict, Formatting.Indented)}");
            _logger.Info($"plainObjectsDict = {JsonConvert.SerializeObject(plainObjectsDict, SerializationHelper.JsonSerializerSettings)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(plainObjectsDict, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        WriteToFile(plainObjectsDict, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private IEnumerable<CustomAttributeData> GetCustomAttributeDataFromProperties(Dictionary<string, IEnumerable<CustomAttributeData>> propertiesAttributesDict, string fieldName)
        {
#if DEBUG
            _logger.Info($"fieldName = {fieldName}");
#endif

            var properyName = GetProperyName(fieldName);

#if DEBUG
            _logger.Info($"properyName = {properyName}");
#endif

            propertiesAttributesDict.TryGetValue(properyName, out var value);

            return value ?? Enumerable.Empty<CustomAttributeData>();
        }

        private string GetProperyName(string fieldName)
        {
#if DEBUG
            _logger.Info($"fieldName = {fieldName}");
#endif

            if(fieldName.EndsWith("k__BackingField"))
            {
                return fieldName.Substring(1, fieldName.IndexOf(">") - 1);
            }

            return string.Empty;
        }

        private object GetSettingsParameter(FieldInfo[] fields, object obj, string settingsParameterName)
        {
#if DEBUG
            _logger.Info($"settingsParameterName = {settingsParameterName}");
#endif

            if(string.IsNullOrWhiteSpace(settingsParameterName))
            {
                return null;
            }

            var field = fields.SingleOrDefault(f => f.Name == settingsParameterName);

            if(field == null)
            {
                return null;
            }

            return field.GetValue(obj);
        }

        private string GetSettingsParameterName(IEnumerable<CustomAttributeData> customAttributes)
        {
            if ((customAttributes?.Count() ?? 0) == 0)
            {
                return null;
            }

            var targetAttribute = customAttributes.SingleOrDefault(p => p.AttributeType == typeof(SocObjectSerializationSettings));

            if(targetAttribute == null)
            {
                return null;
            }

            return targetAttribute.ConstructorArguments.Single().Value as string;
        }

        private bool IsNoSerialiable(IEnumerable<CustomAttributeData> customAttributes)
        {
            if ((customAttributes?.Count() ?? 0) == 0)
            {
                return false;
            }

            var targetAttribute = customAttributes.SingleOrDefault(p => p.AttributeType == typeof(SocNoSerializable));

            return targetAttribute != null;
        }

        private ActionPo CreateActionPlainObject(FieldInfo[] fields, object obj, IEnumerable<CustomAttributeData> customAttributes)
        {
            if ((customAttributes?.Count() ?? 0) == 0)
            {
                return null;
            }

            var targetAttribute = customAttributes.SingleOrDefault(p => p.AttributeType == typeof(SocSerializableActionMember));

            if(targetAttribute == null)
            {
                return null;
            }

#if DEBUG
            _logger.Info($"targetAttribute.AttributeType?.FullName = {targetAttribute.AttributeType?.FullName}");

            foreach (var ctorArg in targetAttribute.ConstructorArguments)
            {
                _logger.Info($"ctorArg.Value = {ctorArg.Value}");
            }

            foreach (var namedArg in targetAttribute.NamedArguments)
            {
                _logger.Info($"namedArg.MemberName = {namedArg.MemberName}");
                _logger.Info($"namedArg.TypedValue.Value = {namedArg.TypedValue.Value}");
            }
#endif

            var fieldName = (string)targetAttribute.ConstructorArguments[0].Value;

#if DEBUG
            _logger.Info($"fieldName = {fieldName}");
#endif

            var field = fields.SingleOrDefault(p => p.Name == fieldName);

            var key = (string)field.GetValue(obj);

#if DEBUG
            _logger.Info($"key = {key}");
#endif

            return new ActionPo
            {
                Key = key,
                Index = (int)targetAttribute.ConstructorArguments[1].Value
            };
        }

        private ObjectPtr NSerializeCustomThreadPool(CustomThreadPool customThreadPool, CustomThreadPoolSerializationSettings settingsParameter, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
#if DEBUG
            _logger.Info($"settingsParameter = {settingsParameter}");
#endif
            
            switch(kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if(!ReferenceEquals(customThreadPool, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            if(settingsParameter == null)
            {
                switch (kindOfSerialization)
                {
                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }

                var foundObject = SearchForMainFieldDeclaration();

                if (foundObject == null)
                {
                    var errorSb = new StringBuilder($"Serialization parameter is required for type {nameof(CustomThreadPool)}.");

                    errorSb.Append(parentObjInfo);

                    throw new ArgumentNullException(nameof(settingsParameter), errorSb.ToString());
                }
                else
                {
                    return foundObject;
                }
            }

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, customThreadPool.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(customThreadPool, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(customThreadPool, targetObject))
                    {
                        _serializationContext.RegObjectPtr(customThreadPool, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
            
            var plainObject = new CustomThreadPoolPo();
            

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    plainObject.Settings = GetSerializedObjectPtr(settingsParameter, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(customThreadPool, targetObject))
                    {
                        plainObject.Settings = GetSerializedObjectPtr(settingsParameter, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(plainObject, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(customThreadPool, targetObject))
                    {
                        WriteToFile(plainObject, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(customThreadPool, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeCancellationTokenSource(CancellationTokenSource cancellationTokenSource, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
#if DEBUG
            _logger.Info($"cancellationTokenSource.IsCancellationRequested = {cancellationTokenSource.IsCancellationRequested}");
#endif
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
            
            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, cancellationTokenSource.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(cancellationTokenSource, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        _serializationContext.RegObjectPtr(cancellationTokenSource, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var plainObject = new CancellationTokenSourcePo();
            plainObject.IsCancelled = cancellationTokenSource.IsCancellationRequested;

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(plainObject, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        WriteToFile(plainObject, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeLinkedCancellationTokenSource(CancellationTokenSource cancellationTokenSource, LinkedCancellationTokenSourceSerializationSettings settingsParameter,
            string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
#if DEBUG
            _logger.Info($"settingsParameter = {settingsParameter}");
            _logger.Info($"cancellationTokenSource.IsCancellationRequested = {cancellationTokenSource.IsCancellationRequested}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            if (settingsParameter == null)
            {
                switch (kindOfSerialization)
                {
                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }

                var foundObject = SearchForMainFieldDeclaration();

                if(foundObject == null)
                {
                    var errorSb = new StringBuilder($"Serialization parameter is required for linked {nameof(CancellationTokenSource)}.");

                    errorSb.Append(parentObjInfo);

                    throw new ArgumentNullException(nameof(settingsParameter), errorSb.ToString());
                }
                else
                {
                    return foundObject;
                }
            }

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, cancellationTokenSource.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(cancellationTokenSource, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        _serializationContext.RegObjectPtr(cancellationTokenSource, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var plainObject = new LinkedCancellationTokenSourcePo();
            plainObject.IsCancelled = cancellationTokenSource.IsCancellationRequested;

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    plainObject.Settings = GetSerializedObjectPtr(settingsParameter, null, parentObjInfo, KindOfSerialization.General, null, rootObj);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        plainObject.Settings = GetSerializedObjectPtr(settingsParameter, null, parentObjInfo, KindOfSerialization.General, null, rootObj);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
            
#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(plainObject, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        WriteToFile(plainObject, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationTokenSource, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeCancellationToken(CancellationToken cancellationToken, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(cancellationToken, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

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
            

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    plainObject.Source = GetSerializedObjectPtr(fieldValue, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationToken, targetObject))
                    {
                        plainObject.Source = GetSerializedObjectPtr(fieldValue, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

#if DEBUG
            _logger.Info($"plainObject = {JsonConvert.SerializeObject(plainObject, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(plainObject, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationToken, targetObject))
                    {
                        WriteToFile(plainObject, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(cancellationToken, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeBareObject(object obj, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(obj, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();

#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, obj.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(obj, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        _serializationContext.RegObjectPtr(obj, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var plainObject = new object();

#if DEBUG
            _logger.Info($"plainObject = {plainObject}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(plainObject, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        WriteToFile(plainObject, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(obj, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionary(IDictionary dictionary, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
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
                        return NSerializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(dictionary, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                    }
                    else
                    {
                        if(SerializationHelper.IsObject(valueGenericParameterType))
                        {
                            return NSerializeGenericDictionaryWithPrimitiveKeyAndObjectValue(dictionary, keyGenericParameterType, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                        }
                        else
                        {
                            return NSerializeGenericDictionaryWithPrimitiveKeyAndCompositeValue(dictionary, keyGenericParameterType, parentObjInfo, kindOfSerialization, targetObject, rootObj);                            
                        }
                    }
                }
                else
                {
                    if(SerializationHelper.IsObject(keyGenericParameterType))
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            return NSerializeGenericDictionaryWithObjectKeyAndPrimitiveValue(dictionary, valueGenericParameterType, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                        }
                        else
                        {
                            if(SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                return NSerializeGenericDictionaryWithObjectKeyAndObjectValue(dictionary, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                            }
                            else
                            {
                                return NSerializeGenericDictionaryWithObjectKeyAndCompositeValue(dictionary, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                            }
                        }
                    }
                    else
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            return NSerializeGenericDictionaryWithCompositeKeyAndPrimitiveValue(dictionary, valueGenericParameterType, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                return NSerializeGenericDictionaryWithCompositeKeyAndObjectValue(dictionary, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                            }
                            else
                            {
                                return NSerializeGenericDictionaryWithCompositeKeyAndCompositeValue(dictionary, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                            }
                        }
                    }
                }
            }

            throw new NotImplementedException("D55AE149-D344-4855-8EC0-2AD18C0F90D5");
        }

        private string GetKeyParentObjInfo(string parentObjInfo)
        {
            return $"{parentObjInfo}.key:";
        }

        private string GetValueParentObjInfo(string parentObjInfo)
        {
            return $"{parentObjInfo}.value:";
        }

        private ObjectPtr NSerializeGenericDictionaryWithCompositeKeyAndCompositeValue(IDictionary dictionary, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(ObjectPtr), typeof(ObjectPtr));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

            var listWithPlainObjects = (IList)Activator.CreateInstance(listWithPlainObjectsType);

            var keyParentObjInfo = GetKeyParentObjInfo(parentObjInfo);
            var valueParentObjInfo = GetValueParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                var plainKey = GetSerializedObjectPtr(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                _logger.Info($"plainKey = {plainKey}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {f
                            if (plainKey == null)
                            {
                                continue;
                            }
                            else
                            {
                                return plainKey;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }

                var plainValue = GetSerializedObjectPtr(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                _logger.Info($"plainValue = {plainValue}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKey, plainValue);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {f
                            if (plainValue == null)
                            {
                                continue;
                            }
                            else
                            {
                                return plainValue;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(listWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithCompositeKeyAndObjectValue(IDictionary dictionary, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(ObjectPtr), typeof(object));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

            var listWithPlainObjects = (IList)Activator.CreateInstance(listWithPlainObjectsType);

            var keyParentObjInfo = GetKeyParentObjInfo(parentObjInfo);
            var valueParentObjInfo = GetValueParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                var plainKey = GetSerializedObjectPtr(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                _logger.Info($"plainKey = {plainKey}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {d
                            if (plainKey == null)
                            {
                                continue;
                            }
                            else
                            {
                                return plainKey;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }

                var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKey, plainValueResult.ConvertedObject);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {f
                            var foundObject = plainValueResult.FoundObject;

                            if (foundObject == null)
                            {
                                continue;
                            }
                            else
                            {
                                return foundObject;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(listWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithCompositeKeyAndPrimitiveValue(IDictionary dictionary, Type valueGenericParameterType,
            string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(ObjectPtr), valueGenericParameterType);

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

            var listWithPlainObjects = (IList)Activator.CreateInstance(listWithPlainObjectsType);

            var keyParentObjInfo = GetKeyParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var plainKey = GetSerializedObjectPtr(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKey = {plainKey}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKey, itemValue);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            var plainKey = GetSerializedObjectPtr(itemKey, null, keyParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKey = {plainKey}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKey, itemValue);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        else
                        {
                            var plainKey = GetSerializedObjectPtr(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKey = {plainKey}");
#endif

                            if (plainKey == null)
                            {
                                continue;
                            }
                            else
                            {
                                return plainKey;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(listWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithObjectKeyAndCompositeValue(IDictionary dictionary, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(object), typeof(ObjectPtr));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

            var listWithPlainObjects = (IList)Activator.CreateInstance(listWithPlainObjectsType);

            var keyParentObjInfo = GetKeyParentObjInfo(parentObjInfo);
            var valueParentObjInfo = GetValueParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                (object ConvertedObject, ObjectPtr FoundObject) plainKeyResult;

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                        _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif
                        }
                        else
                        {
                            plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif

                            var foundObject = plainKeyResult.FoundObject;

                            if (foundObject == null)
                            {
                                continue;
                            }
                            else
                            {
                                return foundObject;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var plainValue = GetSerializedObjectPtr(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValue = {plainValue}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKeyResult.ConvertedObject, plainValue);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            var plainValue = GetSerializedObjectPtr(itemValue, null, valueParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValue = {plainValue}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKeyResult.ConvertedObject, plainValue);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        else
                        {
                            var plainValue = GetSerializedObjectPtr(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValue = {plainValue}");
#endif

                            if (plainValue == null)
                            {
                                continue;
                            }
                            else
                            {
                                return plainValue;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(listWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithObjectKeyAndObjectValue(IDictionary dictionary, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(object), typeof(object));

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

            var listWithPlainObjects = (IList)Activator.CreateInstance(listWithPlainObjectsType);

            var keyParentObjInfo = GetKeyParentObjInfo(parentObjInfo);
            var valueParentObjInfo = GetValueParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                (object ConvertedObject, ObjectPtr FoundObject) plainKeyResult;

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                        _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif
                        }
                        else
                        {
                            plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif

                            var foundObject = plainKeyResult.FoundObject;

                            if (foundObject == null)
                            {
                                continue;
                            }
                            else
                            {
                                return foundObject;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKeyResult.ConvertedObject, plainValueResult.ConvertedObject);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, null, valueParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKeyResult.ConvertedObject, plainValueResult.ConvertedObject);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        else
                        {
                            var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                            var foundObject = plainValueResult.FoundObject;

                            if (foundObject == null)
                            {
                                continue;
                            }
                            else
                            {
                                return foundObject;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(listWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithObjectKeyAndPrimitiveValue(IDictionary dictionary, Type valueGenericParameterType,
            string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(object), valueGenericParameterType);

            var listWithPlainObjectsType = typeof(List<>).MakeGenericType(keyValuePairType);

            var listWithPlainObjects = (IList)Activator.CreateInstance(listWithPlainObjectsType);

            var keyParentObjInfo = GetKeyParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKeyResult.ConvertedObject, itemValue);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            var plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif

                            var keyValuePair = Activator.CreateInstance(keyValuePairType, plainKeyResult.ConvertedObject, itemValue);

#if DEBUG
                            _logger.Info($"keyValuePair = {keyValuePair}");
#endif

                            listWithPlainObjects.Add(keyValuePair);
                        }
                        else
                        {
                            var plainKeyResult = ConvertObjectCollectionValueToSerializableFormat(itemKey, null, keyParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainKeyResult = {plainKeyResult}");
#endif

                            var foundObject = plainKeyResult.FoundObject;

                            if (foundObject == null)
                            {
                                continue;
                            }
                            else
                            {
                                return foundObject;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, SerializationHelper.JsonSerializerSettings)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(listWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithPrimitiveKeyAndCompositeValue(IDictionary dictionary, Type keyGenericParameterType,
            string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var dictWithPlainObjectsType = typeof(Dictionary<,>).MakeGenericType(keyGenericParameterType, typeof(ObjectPtr));

            var dictWithPlainObjects = (IDictionary)Activator.CreateInstance(dictWithPlainObjectsType);

            var valueParentObjInfo = GetValueParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var plainValue = GetSerializedObjectPtr(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValue = {plainValue}");
#endif

                            dictWithPlainObjects[itemKey] = plainValue;
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            var plainValue = GetSerializedObjectPtr(itemValue, null, valueParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValue = {plainValue}");
#endif

                            dictWithPlainObjects[itemKey] = plainValue;
                        }
                        else
                        {
                            var plainValue = GetSerializedObjectPtr(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValue = {plainValue}");
#endif

                            if (plainValue == null)
                            {
                                continue;
                            }
                            else
                            {
                                return plainValue;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"dictWithPlainObjects = {JsonConvert.SerializeObject(dictWithPlainObjects, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(dictWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(dictWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithPrimitiveKeyAndObjectValue(IDictionary dictionary, Type keyGenericParameterType,
            string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var dictWithPlainObjectsType = typeof(Dictionary<,>).MakeGenericType(keyGenericParameterType, typeof(object));

            var dictWithPlainObjects = (IDictionary)Activator.CreateInstance(dictWithPlainObjectsType);

            var valueParentObjInfo = GetValueParentObjInfo(parentObjInfo);

            foreach (DictionaryEntry item in dictionary)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;

#if DEBUG
                _logger.Info($"itemKey = {itemKey}");
                _logger.Info($"itemValue = {itemValue}");
#endif

                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                            dictWithPlainObjects[itemKey] = plainValueResult.ConvertedObject;
                        }
                        
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(dictionary, targetObject))
                        {
                            var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, null, valueParentObjInfo, KindOfSerialization.General, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                            dictWithPlainObjects[itemKey] = plainValueResult.ConvertedObject;
                        }
                        else
                        {
                            var plainValueResult = ConvertObjectCollectionValueToSerializableFormat(itemValue, null, valueParentObjInfo, kindOfSerialization, targetObject, rootObj);

#if DEBUG
                            _logger.Info($"plainValueResult = {plainValueResult}");
#endif

                            var foundObject = plainValueResult.FoundObject;

                            if (foundObject == null)
                            {
                                continue;
                            }
                            else
                            {
                                return foundObject;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"dictWithPlainObjects = {JsonConvert.SerializeObject(dictWithPlainObjects, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(dictWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(dictWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericDictionaryWithPrimitiveKeyAndPrimitiveValue(IDictionary dictionary,
            string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(dictionary, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, dictionary.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        _serializationContext.RegObjectPtr(dictionary, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

#if DEBUG
            _logger.Info($"dictionary = {JsonConvert.SerializeObject(dictionary, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(dictionary, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        WriteToFile(dictionary, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(dictionary, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeGenericList(IEnumerable enumerable, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
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
                    return NSerializeListWithPrimitiveParameter(enumerable, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                }

                if (SerializationHelper.IsObject(genericParameterType))
                {
                    return NSerializeListWithObjectParameter(enumerable, parentObjInfo, kindOfSerialization, targetObject, rootObj);
                }
            }

            return NSerializeListWithCompositeParameter(enumerable, parentObjInfo, kindOfSerialization, targetObject, rootObj);
        }

        private ObjectPtr NSerializeListWithCompositeParameter(IEnumerable enumerable, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(enumerable, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

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

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(enumerable, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        _serializationContext.RegObjectPtr(enumerable, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var listWithPlainObjects = new List<ObjectPtr>();

            foreach (var item in enumerable)
            {
                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var itemObjectPtr = GetSerializedObjectPtr(item, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                            listWithPlainObjects.Add(itemObjectPtr);
                        }                        
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(enumerable, targetObject))
                        {
                            var itemObjectPtr = GetSerializedObjectPtr(item, null, parentObjInfo, KindOfSerialization.General, targetObject, rootObj);

                            listWithPlainObjects.Add(itemObjectPtr);
                        }
                        else
                        {
                            var itemObjectPtr = GetSerializedObjectPtr(item, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                            if (itemObjectPtr == null)
                            {
                                continue;
                            }
                            else
                            {
                                return itemObjectPtr;
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeListWithObjectParameter(IEnumerable enumerable, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(enumerable, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

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

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(enumerable, objectPtr);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        _serializationContext.RegObjectPtr(enumerable, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var listWithPlainObjects = new List<object>();

            foreach (var item in enumerable)
            {
                switch (kindOfSerialization)
                {
                    case KindOfSerialization.General:
                        {
                            var itemResult = ConvertObjectCollectionValueToSerializableFormat(item, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                            listWithPlainObjects.Add(itemResult.ConvertedObject);
                        }
                        break;

                    case KindOfSerialization.Searching:
                        if (ReferenceEquals(enumerable, targetObject))
                        {
                            var itemResult = ConvertObjectCollectionValueToSerializableFormat(item, null, parentObjInfo, KindOfSerialization.General, targetObject, rootObj);

                            listWithPlainObjects.Add(itemResult.ConvertedObject);
                        }
                        else
                        {
                            var itemResult = ConvertObjectCollectionValueToSerializableFormat(item, null, parentObjInfo, kindOfSerialization, targetObject, rootObj);

                            var foundObject = itemResult.FoundObject;

                            if(foundObject == null)
                            {
                                continue;
                            }

                            return foundObject;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
                }
            }

#if DEBUG
            _logger.Info($"listWithPlainObjects = {JsonConvert.SerializeObject(listWithPlainObjects, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(listWithPlainObjects, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        WriteToFile(listWithPlainObjects, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }
        }

        private ObjectPtr NSerializeListWithPrimitiveParameter(IEnumerable enumerable, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    break;

                case KindOfSerialization.Searching:
                    if (!ReferenceEquals(enumerable, targetObject))
                    {
                        return null;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            var instanceId = CreateInstanceId();
            
#if DEBUG
            _logger.Info($"instanceId = {instanceId}");
#endif

            var objectPtr = new ObjectPtr(instanceId, enumerable.GetType().FullName);

#if DEBUG
            _logger.Info($"objectPtr = {objectPtr}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    _serializationContext.RegObjectPtr(enumerable, objectPtr);
                    break;

                    case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        _serializationContext.RegObjectPtr(enumerable, objectPtr);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

#if DEBUG
            _logger.Info($"enumerable = {JsonConvert.SerializeObject(enumerable, Formatting.Indented)}");
#endif

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    WriteToFile(enumerable, instanceId);
                    break;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        WriteToFile(enumerable, instanceId);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }

            switch (kindOfSerialization)
            {
                case KindOfSerialization.General:
                    return objectPtr;

                case KindOfSerialization.Searching:
                    if (ReferenceEquals(enumerable, targetObject))
                    {
                        return objectPtr;
                    }
                    else
                    {
                        return null;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, null);
            }            
        }

        private (object ConvertedObject, ObjectPtr FoundObject) ConvertObjectCollectionValueToSerializableFormat(object value, object settingsParameter, string parentObjInfo, KindOfSerialization kindOfSerialization, object targetObject, object rootObj)
        {
            if(value == null)
            {
                return (null, null);
            }

            if (SerializationHelper.IsPrimitiveType(value))
            {
                return (value, null);
            }

            var objPtr = GetSerializedObjectPtr(value, settingsParameter, parentObjInfo, kindOfSerialization, targetObject, rootObj);

            return (objPtr, objPtr);
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
