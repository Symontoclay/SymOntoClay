using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymOntoClay.Serialization.Implementation
{
    public class PreDeserializeItemsCollector
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public PreDeserializeItemsCollector(IDeserializationContext deserializationContext)
        {
            _deserializationContext = deserializationContext;
        }

        private readonly IDeserializationContext _deserializationContext;

        private List<object> _visitedObjects = new List<object>();

        public void Run(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            if(obj == null)
            {
                return;
            }

            if(_visitedObjects.Contains(obj))
            {
                return;
            }

            _visitedObjects.Add(obj);

            var type = obj.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if (type.FullName.StartsWith("System.Action"))
            {
                return;
            }

            if (type.FullName.StartsWith("System.Func"))
            {
                return;
            }

            switch (type.FullName)
            {
                case "System.Object":
                    return;

                case "System.Threading.CancellationTokenSource":
                    return;

                case "System.Threading.CancellationTokenSource+Linked1CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+Linked2CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+LinkedNCancellationTokenSource":
                    return;

                case "System.Threading.CancellationToken":
                    return;

                case "System.Threading.ManualResetEvent":
                    return;

                case "SymOntoClay.Threading.CustomThreadPool":
                    return;
            }

            switch (type.Name)
            {
                case "List`1":
                    NRunGenericList((IEnumerable)obj);
                    break;

                case "Stack`1":
                    NRunGenericStack((IEnumerable)obj);
                    break;

                case "Queue`1":
                    NRunGenericQueue((IEnumerable)obj);
                    break;

                case "Dictionary`2":
                    NRunGenericDictionary((IDictionary)obj);
                    break;

                default:
                    if (type.FullName.StartsWith("System.Threading.") ||
                        type.FullName.StartsWith("System.Collections."))
                    {
                        throw new NotImplementedException("303F2A8B-1EE0-476E-BDB2-027A4BB4249F");
                    }

                    NRunComposite(obj);
                    break;
            }
        }

        private void NRunComposite(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var type = obj.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

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

                var customAttributeDataFromProperties = SerializationHelper.GetCustomAttributeDataFromProperties(propertiesAttributesDict, field.Name);

#if DEBUG
                _logger.Info($"customAttributeDataFromProperties?.Count() = {customAttributeDataFromProperties?.Count()}");
#endif

                if (customAttributeDataFromProperties?.Any() ?? false)
                {
                    customAttributes = customAttributes.Concat(customAttributeDataFromProperties);
                }

                var settingsInstanceId = GetSettingsInstanceId(customAttributes);

#if DEBUG
                _logger.Info($"settingsInstanceId = {settingsInstanceId}");
#endif

                if(settingsInstanceId != null)
                {
                    var externalSettings = field.GetValue(obj);

#if DEBUG
                    _logger.Info($"externalSettings = {externalSettings}");
#endif

                    _deserializationContext.RegExternalSettings(externalSettings, field.FieldType, type, settingsInstanceId);
                }
            }
        }

        private string GetSettingsInstanceId(IEnumerable<CustomAttributeData> customAttributes)
        {
            if ((customAttributes?.Count() ?? 0) == 0)
            {
                return null;
            }

            var targetAttribute = customAttributes.SingleOrDefault(p => p.AttributeType == typeof(SocSerializableExternalSettings));

            if (targetAttribute == null)
            {
                return null;
            }

            return targetAttribute.ConstructorArguments.Single().Value as string;
        }

        private void NRunGenericList(IEnumerable enumerable)
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
                    return;
                }

                if (SerializationHelper.IsObject(genericParameterType))
                {
                    return;
                }

                NRunListWithCompositeParameter(enumerable);
                return;
            }

            throw new NotImplementedException("4D26127E-EA35-4547-B0F1-06B37CE3A8D6");
        }

        private void NRunListWithCompositeParameter(IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                Run(item);
            }
        }

        private void NRunGenericStack(IEnumerable enumerable)
        {
            var type = enumerable.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if (type.IsGenericType)
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
                    return;
                }

                if (SerializationHelper.IsObject(genericParameterType))
                {
                    return;
                }

                NRunGenericStackWithCompositeParameter(enumerable);
                return;
            }

            throw new NotImplementedException("F838887E-6807-4D14-9D29-C0C607A283BC");
        }

        private void NRunGenericStackWithCompositeParameter(IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                Run(item);
            }
        }

        private void NRunGenericQueue(IEnumerable enumerable)
        {
            var type = enumerable.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if (type.IsGenericType)
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
                    return;
                }

                if (SerializationHelper.IsObject(genericParameterType))
                {
                    return;
                }

                NRunGenericQueueWithCompositeParameter(enumerable);
                return;
            }

            throw new NotImplementedException("89B85280-4757-4F99-B0A8-9225BB449CA6");
        }

        private void NRunGenericQueueWithCompositeParameter(IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                Run(item);
            }
        }

        private void NRunGenericDictionary(IDictionary dictionary)
        {
            var type = dictionary.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

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
                        return;
                    }
                    else
                    {
                        if (SerializationHelper.IsObject(valueGenericParameterType))
                        {
                            return;
                        }
                        else
                        {
                            NRunGenericDictionaryWithPrimitiveKeyAndCompositeValue(dictionary);
                            return;
                        }
                    }
                }
                else
                {
                    if (SerializationHelper.IsObject(keyGenericParameterType))
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            return;
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                return;
                            }
                            else
                            {
                                NRunGenericDictionaryWithObjectKeyAndCompositeValue(dictionary);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (SerializationHelper.IsPrimitiveType(valueGenericParameterType))
                        {
                            NRunGenericDictionaryWithCompositeKeyAndPrimitiveValue(dictionary);
                            return;
                        }
                        else
                        {
                            if (SerializationHelper.IsObject(valueGenericParameterType))
                            {
                                NRunGenericDictionaryWithCompositeKeyAndObjectValue(dictionary);
                                return;
                            }
                            else
                            {
                                NRunGenericDictionaryWithCompositeKeyAndCompositeValue(dictionary);
                                return;
                            }
                        }
                    }
                }
            }

            throw new NotImplementedException("3EB16C9A-C16B-4EDF-8DBC-EC0983CBC826");
        }

        private void NRunGenericDictionaryWithPrimitiveKeyAndCompositeValue(IDictionary dictionary)
        {
            foreach (DictionaryEntry item in dictionary)
            {
                Run(item.Value);
            }
        }

        private void NRunGenericDictionaryWithObjectKeyAndCompositeValue(IDictionary dictionary)
        {
            foreach (DictionaryEntry item in dictionary)
            {
                Run(item.Value);
            }
        }

        private void NRunGenericDictionaryWithCompositeKeyAndPrimitiveValue(IDictionary dictionary)
        {
            foreach (DictionaryEntry item in dictionary)
            {
                Run(item.Key);
            }
        }

        private void NRunGenericDictionaryWithCompositeKeyAndObjectValue(IDictionary dictionary)
        {
            foreach (DictionaryEntry item in dictionary)
            {
                Run(item.Key);
            }
        }

        private void NRunGenericDictionaryWithCompositeKeyAndCompositeValue(IDictionary dictionary)
        {
            foreach (DictionaryEntry item in dictionary)
            {
                Run(item.Key);
                Run(item.Value);
            }
        }
    }
}
