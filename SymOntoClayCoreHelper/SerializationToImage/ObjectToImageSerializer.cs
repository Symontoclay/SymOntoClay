using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectToImageSerializer: BaseObjectSerializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif
        
        public ObjectToImageSerializer(ISerializedObjectsPool serializedObjectsPool, IStructuralContext structuralContext, IDataCardWriter dataCardWriter)
            : base(serializedObjectsPool)
        {
            _structuralContext = structuralContext;
            _dataCardWriter = dataCardWriter;
        }

        private readonly IStructuralContext _structuralContext;
        private readonly IDataCardWriter _dataCardWriter;

        private HashSet<object> _visitedObjects = new HashSet<object>();

        /// <inheritdoc/>
        protected override bool TryGetSerializedValue(object obj, out SerializedValue serializedValue)
        {
#if DEBUG
            _logger.Info($"obj?.GetType()?.FullName = {obj?.GetType()?.FullName}");
#endif

            if(obj == null)
            {
                return base.TryGetSerializedValue(obj, out serializedValue);
            }

            var type = obj.GetType();

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            _logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
#endif

            switch (kindOfStructuralObject)
            {
                case KindOfStructuralObject.WorldRoot:
                case KindOfStructuralObject.WorldComponent:
                    {
                        if (!_visitedObjects.Contains(obj))
                        {
                            serializedValue = null;

                            return false;
                        }

                        return base.TryGetSerializedValue(obj, out serializedValue);
                    }

                case KindOfStructuralObject.WorldSettings:
                case KindOfStructuralObject.UsualObject:
                case KindOfStructuralObject.SerializeWithSerializationDataCreation:
                    return base.TryGetSerializedValue(obj, out serializedValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStructuralObject), kindOfStructuralObject, "E710D2D8-1795-4EEC-B9D1-D79069D75534");
            }
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeBareObject(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ClassCard()
            {
                Header = serializedValue
            };

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericList(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ListCard()
            {
                Header = serializedValue
            };

            var enumerable = (IEnumerable)obj;

            var items = new List<SerializedValue>();

            foreach ( var item in enumerable)
            {
#if DEBUG
                _logger.Info($"item = {item}");
#endif

                var fieldSerializedValue = SerializeValue(item, string.Empty);

#if DEBUG
                _logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

                items.Add(fieldSerializedValue);
            }

            card.Items = items;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            throw new NotImplementedException("C7F00063-BE35-44CD-9528-32C3600EF75E");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"B55F1AE6-7670-4158-84AC-20053293DD4A: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C577B505-79EB-4EB0-81D6-CEE7E181C31D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"E9247CFC-4ADD-4F14-9D3B-EA5BB918A2FB: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C3BE6016-0DA1-4238-BB7E-C12668369925");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"567F2A4C-26BD-4ECB-81CA-5A09C06C4D8A: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C59FDA1A-7C7B-4E67-A6F4-B0507CA6E2DF");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"FA40B5E6-45E9-4945-8B24-7409410975B1: please check type '{type.FullName}'");
            }
#endif

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            _logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
#endif

            switch (kindOfStructuralObject)
            {
                case KindOfStructuralObject.WorldRoot:
                case KindOfStructuralObject.WorldComponent:
                    return SerializeKeyWorldComponent(obj, type, path);

                case KindOfStructuralObject.SerializeWithSerializationDataCreation:
                    return SerializeWithSerializationDataCreation(obj, type, path);

                case KindOfStructuralObject.UsualObject:
                    return SerializeUsualObject(obj, type, path);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStructuralObject), kindOfStructuralObject, "DC2F6C00-5475-4FE3-B9C3-96E1C523B124");
            }
        }

        private SerializedValue SerializeKeyWorldComponent(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var fieldsWithSerializedMembers = GetFields(type)
                .Where(f => f.IsDefined(typeof(SerializedMemberAttribute), false))
                .ToList();

#if DEBUG
            _logger.Info($"fieldsWithSerializedMembers.Count = {fieldsWithSerializedMembers.Count}");
#endif

            foreach (var field in fieldsWithSerializedMembers)
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
#endif

                var fieldValue = field.GetValue(obj);

#if DEBUG
                _logger.Info($"fieldValue = {fieldValue}");
#endif

                var fieldSerializedValue = SerializeValue(fieldValue, path);

#if DEBUG
                _logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif
            }

            var fieldsWithChildren = GetFields(type)
                .Where(f => f.IsDefined(typeof(SerializedMemberWithChildrenAttribute), false))
                .ToList();

#if DEBUG
            _logger.Info($"fieldsWithChildren.Count = {fieldsWithChildren.Count}");
#endif

            foreach(var field in fieldsWithChildren)
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
#endif

                var fieldValue = field.GetValue(obj);

#if DEBUG
                _logger.Info($"fieldValue = {fieldValue}");
#endif

                var fieldSerializedValue = SerializeValue(fieldValue, path);

#if DEBUG
                _logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif
            }

            throw new NotImplementedException("C34FFEC2-FEC7-412A-BF4E-985ACF39DEA8");
        }

        private SerializedValue SerializeWithSerializationDataCreation(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif
            
            var card = new ClassCardWithSerializationData()
            {
                Header = serializedValue
            };

            var serializationDataFactory = (ISerializationDataFactory)obj;

            var serializationData = serializationDataFactory.GetSerializationData();

#if DEBUG
            _logger.Info($"serializationData = {serializationData}");
#endif

            var serializedSerializationDataValue = SerializeValue(serializationData, string.Empty);

#if DEBUG
            _logger.Info($"serializedSerializationDataValue = {serializedSerializationDataValue}");
#endif

            card.SerializationData = serializedSerializationDataValue;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        private SerializedValue SerializeUsualObject(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ClassCard()
            {
                Header = serializedValue
            };

            var fields = GetFields(type);

#if DEBUG
            _logger.Info($"fields.Count() = {fields.Count()}");
#endif

            var cardFieldDict = new Dictionary<string, SerializedValue>();

            foreach (var field in fields)
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
#endif

                if (field.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

                ProcessFieldInfo(field, obj, cardFieldDict);
            }

            card.Fields = cardFieldDict;

            var cardPropertyDict = new Dictionary<string, SerializedValue>();

            var propertyInfos = GetProperties(type);

#if DEBUG
            _logger.Info($"propertyInfos.Count() = {propertyInfos.Count()}");
#endif

            foreach (var property in propertyInfos)
            {
#if DEBUG
                _logger.Info($"property.Name = {property.Name}");
#endif

                if (property.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

                ProcessPropertyInfo(property, obj, cardPropertyDict);
            }

            card.Properties = cardPropertyDict;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        private void ProcessFieldInfo(FieldInfo field, object obj, Dictionary<string, SerializedValue> cardFieldDict)
        {
            var fieldValue = field.GetValue(obj);

#if DEBUG
            _logger.Info($"fieldValue = {fieldValue}");
#endif

            var fieldSerializedValue = SerializeValue(fieldValue, string.Empty);

#if DEBUG
            _logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

            cardFieldDict[field.Name] = fieldSerializedValue;
        }

        private void ProcessPropertyInfo(PropertyInfo property, object obj, Dictionary<string, SerializedValue> cardPropertyDict)
        {
            var propertyValue = property.GetValue(obj);

#if DEBUG
            _logger.Info($"propertyValue = {propertyValue}");
#endif

            var serializeValueMode = SerializeValueMode.General;

            if (property.IsDefined(typeof(MemberWithExternalValueAttribute), true))
            {
                serializeValueMode = SerializeValueMode.ExternalValue;
            }

            var propertySerializedValue = SerializeValue(propertyValue, string.Empty, serializeValueMode);

#if DEBUG
            _logger.Info($"propertySerializedValue = {propertySerializedValue}");
#endif

            cardPropertyDict[property.Name] = propertySerializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeManualResetEvent(object obj, Type type, string path)
        {
            throw new NotImplementedException("C3083A33-361D-4786-AE17-0546BE153C9D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj, Type type)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"184ECF5C-C554-47B9-BD3B-272CE9BAD9B4: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C741439E-BC15-4F4C-8F0A-C775975A3863");
        }

        private readonly List<string> _tmpProcessedTypes = new List<string>()
        {
            "SymOntoClay.UnityAsset.Core.World.WorldCore",
            "SymOntoClay.UnityAsset.Core.Internal.WorldContext",
            "SymOntoClay.UnityAsset.Core.Internal.SerializedWorldContext",
            "SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache.LogicQueryParseAndCache",
            "SymOntoClay.Core.Internal.BaseCoreContext",
            "SymOntoClay.Monitor.Internal.MonitorNode",
            "SymOntoClay.Monitor.Internal.SerializationData.MonitorNodeSerializationData",
            "SymOntoClay.UnityAsset.Core.Internal.DateAndTime.DateTimeProvider"
        };
    }
}
