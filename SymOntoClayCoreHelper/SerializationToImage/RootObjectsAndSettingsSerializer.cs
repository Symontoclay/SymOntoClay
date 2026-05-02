using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class RootObjectsAndSettingsSerializer : BaseObjectSerializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public RootObjectsAndSettingsSerializer(ISerializedObjectsPool serializedObjectsPool, IStructuralContext structuralContext, IDataCardWriter dataCardWriter)
            : base(serializedObjectsPool)
        {
            _structuralContext = structuralContext;
            _dataCardWriter = dataCardWriter;
        }

        private readonly IStructuralContext _structuralContext;
        private readonly IDataCardWriter _dataCardWriter;

        /// <inheritdoc/>
        protected override SerializedValue SerializeBareObject(object obj, Type type, string path)
        {
            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalClassCard()
            {
                Header = serializedValue,
                Path = path
            };

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(KindOfDataCard.ExternalClassCard, card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericList(object obj, Type type, string path)
        {
            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalListCard()
            {
                Header = serializedValue,
                Path = path
            };

            var enumerable = (IEnumerable)obj;

            var items = new List<SerializedValue>();

            foreach (var item in enumerable) 
            {
#if DEBUG
                _logger.Info($"item = {item}");
#endif

                var fieldPath = $"{path}/[*]";

#if DEBUG
                _logger.Info($"fieldPath = {fieldPath}");
#endif

                var fieldSerializedValue = SerializeValue(item, fieldPath);

#if DEBUG
                _logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

                items.Add(fieldSerializedValue);
            }

            card.Items = items;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(KindOfDataCard.ExternalListCard, card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"4330F2DE-ED81-4790-9089-E3094E76C6B4: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C81F1DFE-FACE-4AFA-BD4A-ED501B903D3F");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"590B9CF6-26F9-4E25-BEA2-9F64B836B53D: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C925989B-E02A-469D-9703-C73A22E7491D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"73ED2E1B-CF3C-4F38-A818-D5EBA81566EB: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C6176FA0-9C26-4183-B80B-3A3D7E0D873F");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"EBD4F7D6-9027-4385-B9CE-46064B264766: please check type '{type.FullName}'");
            }
#endif

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            _logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
#endif

            switch(kindOfStructuralObject)
            {
                case KindOfStructuralObject.WorldRoot:
                    return SerializeWorldRoot(obj, type, path);

                case KindOfStructuralObject.WorldSettings:
                    return SerializeSettings(obj, type, path);

                case KindOfStructuralObject.UsualObject:
                    return SerializeUsualObject(obj, type, path);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStructuralObject), kindOfStructuralObject, "5A89589A-C1E1-4133-BFAE-9BE1FA882427");
            }
        }

        private SerializedValue SerializeWorldRoot(object obj, Type type, string path)
        {
#if DEBUG
            _logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.IsPreregistered);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var cardFieldDict = new Dictionary<string, SerializedValue>();

            var fieldsWithSettings = GetFields(type)
                .Where(f => f.IsDefined(typeof(SettingsMemberAttribute), false))
                .ToList();

#if DEBUG
            _logger.Info($"fieldsWithSettings.Count = {fieldsWithSettings.Count}");
#endif

            foreach(var field in fieldsWithSettings)
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
#endif

                ProcessFieldInfo(field, obj, path, cardFieldDict);
            }

            var fields = GetFields(type)
                //.Where(f => f.IsDefined(typeof(SettingsMemberAttribute), false))
                .ToList();

#if DEBUG
            _logger.Info($"fields.Count = {fields.Count}");
#endif

            foreach(var field in fields)
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
#endif

                if(field.IsDefined(typeof(SerializedMemberAttribute), false))
                {
                    continue;
                }

                if (field.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

                ProcessFieldInfo(field, obj, path, cardFieldDict);
            }

            var properties = GetProperties(type);

#if DEBUG
            _logger.Info($"properties.Length = {properties.Count()}");
#endif

            if(properties.Any())
            {
                throw new NotImplementedException("CFD2C27A-2E72-40D7-A365-3A6BFE4467CF");
            }

            //throw new NotImplementedException("C90EDF11-865C-4725-ABA4-A803814DC014");

            return serializedValue;
        }

        private SerializedValue SerializeSettings(object obj, Type type, string path)
        {
#if DEBUG
            _logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalClassCard()
            {
                Header = serializedValue,
                Path = path
            };

            var fields = GetFields(type);

            if(fields.Any())
            {
                throw new NotImplementedException("B71D9BC6-FF5E-4621-8665-599B2685B0BE");
            }

            var cardPropertyDict = new Dictionary<string, SerializedValue>();

            var properties = GetProperties(type);

#if DEBUG
            _logger.Info($"properties.Count() = {properties.Count()}");
#endif

            foreach (var property in properties)
            {
#if DEBUG
                _logger.Info($"property.Name = {property.Name}");
#endif

                if (property.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

                ProcessPropertyInfo(property, obj, path, cardPropertyDict);
            }

            card.Properties = cardPropertyDict;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(KindOfDataCard.ExternalClassCard, card);

            //throw new NotImplementedException("C9EBF62E-2FB3-4241-A9BB-E70A5D6A0774");

            return serializedValue;
        }

        private SerializedValue SerializeUsualObject(object obj, Type type, string path)
        {
#if DEBUG
            _logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalClassCard()
            {
                Header = serializedValue,
                Path = path
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

                ProcessFieldInfo(field, obj, path, cardFieldDict);
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

                ProcessPropertyInfo(property, obj, path, cardPropertyDict);
            }

            card.Properties = cardPropertyDict;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(KindOfDataCard.ExternalClassCard, card);

            //throw new NotImplementedException("C871A015-857B-4C95-B5AA-A4DCAB10EB43");

            return serializedValue;
        }

        private void ProcessFieldInfo(FieldInfo field, object obj, string path, Dictionary<string, SerializedValue> cardFieldDict)
        {
            var fieldValue = field.GetValue(obj);

#if DEBUG
            _logger.Info($"fieldValue = {fieldValue}");
#endif

            var fieldPath = $"{path}/{field.Name}";

#if DEBUG
            _logger.Info($"fieldPath = {fieldPath}");
#endif

            var fieldSerializedValue = SerializeValue(fieldValue, fieldPath);

#if DEBUG
            _logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

            cardFieldDict[field.Name] = fieldSerializedValue;
        }

        private void ProcessPropertyInfo(PropertyInfo property, object obj, string path, Dictionary<string, SerializedValue> cardPropertyDict)
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

            var propertyPath = $"{path}/{property.Name}";

#if DEBUG
            _logger.Info($"propertyPath = {propertyPath}");
#endif

            var propertySerializedValue = SerializeValue(propertyValue, propertyPath, serializeValueMode);

#if DEBUG
            _logger.Info($"propertySerializedValue = {propertySerializedValue}");
#endif

            cardPropertyDict[property.Name] = propertySerializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeManualResetEvent(object obj, Type type, string path)
        {
#if DEBUG
            _logger.Info($"path = {path}");
#endif

            var autoResetEvent = (ManualResetEvent)obj;

            var isSet = autoResetEvent.WaitOne(0);

#if DEBUG
            _logger.Info($"isSet = {isSet}");
#endif

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalManualResetEventClassCard()
            {
                Header = serializedValue,
                Path = path,
                IsSet = isSet
            };

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(KindOfDataCard.ExternalManualResetEventClassCard, card);


            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj, Type type)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"FC8ED02D-2489-43ED-A10E-DDC32406325F: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C6866BBD-6E0A-46CD-BBCA-2D9B66381B2B");
        }

        private readonly List<string> _tmpProcessedTypes = new List<string>()
        {
            "TestSandbox.SerializationToImage.TstWorldContext",
            "SymOntoClay.UnityAsset.Core.WorldSettings",
            "SymOntoClay.UnityAsset.Core.Internal.WorldContext",
            "SymOntoClay.Core.ThreadingSettings",
            "SymOntoClay.Threading.CustomThreadPoolSettings",
            "SymOntoClay.Common.Cancellation.CancellationTokenSourceContext",
            "SymOntoClay.Common.Cancellation.CancellationLinkedTokenSourceContext",
            "SymOntoClay.Common.Cancellation.CancellationTokenContext",
            "SymOntoClay.UnityAsset.Core.Internal.Threads.ThreadsCoreComponent",
            "SymOntoClay.ActiveObject.Threads.ActiveObjectCommonContext"
        };
    }
}
