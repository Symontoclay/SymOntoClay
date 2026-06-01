using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.ComponentsInterfaces;
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

        public RootObjectsAndSettingsSerializer(ISerializedObjectsPool serializedObjectsPool, ISerializedTypesPool serializedTypesPool, IStructuralContext structuralContext, IDataCardWriter dataCardWriter)
            : base(serializedObjectsPool, serializedTypesPool)
        {
            _structuralContext = structuralContext;
            _dataCardWriter = dataCardWriter;

#if DEBUG
            InitTmpProcessedMembersOfTypes();
#endif
        }

        private readonly IStructuralContext _structuralContext;
        private readonly IDataCardWriter _dataCardWriter;

        private HashSet<object> _visitedObjects = new HashSet<object>();

        /// <inheritdoc/>
        protected override bool TryGetSerializedValue(object obj, out SerializedValue serializedValue)
        {
#if DEBUG
            //_logger.Info($"obj?.GetType()?.FullName = {obj?.GetType()?.FullName}");
#endif

            if(!_visitedObjects.Contains(obj))
            {
                serializedValue = null;

                return false;
            }

            return base.TryGetSerializedValue(obj, out serializedValue);
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeExternalValue(object obj)
        {
            return _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.ExternalValue);
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeReflectionType(object obj, Type type)
        {
            throw new NotImplementedException("C18526FF-CEE4-4BBF-828D-CC39965A5E99");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeBareObject(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalClassCard()
            {
                Header = serializedValue,
                Path = path
            };

#if DEBUG
            //_logger.Info($"card = {card}");
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
            //_logger.Info($"serializedValue = {serializedValue}");
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
                //_logger.Info($"item = {item}");
#endif

                var fieldPath = $"{path}/[*]";

#if DEBUG
                //_logger.Info($"fieldPath = {fieldPath}");
#endif

                var fieldSerializedValue = SerializeValue(item, fieldPath);

#if DEBUG
                //_logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

                items.Add(fieldSerializedValue);
            }

            card.Items = items;

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj, Type type, string path)
        {
#if DEBUG
            TmpCheckProcessedTypes("1CBE32CC-957C-4E43-A548-EE3705613153", type);
#endif

            throw new NotImplementedException("C81F1DFE-FACE-4AFA-BD4A-ED501B903D3F");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
#if DEBUG
            TmpCheckProcessedTypes("5052BA76-14D6-436B-89C7-3910FDB46920", type);
#endif

            throw new NotImplementedException("C925989B-E02A-469D-9703-C73A22E7491D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
#if DEBUG
            TmpCheckProcessedTypes("4923A521-BE0E-4ACC-8AF9-CCD1D78BA117", type);
#endif

            throw new NotImplementedException("C6176FA0-9C26-4183-B80B-3A3D7E0D873F");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj, Type type, string path)
        {
#if DEBUG
            TmpCheckProcessedTypes("BFC68775-0E7F-4EF9-8393-9AFD8ED57344", type);
#endif

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            //_logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
#endif

            switch(kindOfStructuralObject)
            {
                case KindOfStructuralObject.WorldRoot:
                case KindOfStructuralObject.WorldComponent:
                    return SerializeKeyWorldComponent(obj, type, path);

                case KindOfStructuralObject.WorldSettings:
                    return SerializeSettings(obj, type, path);

                case KindOfStructuralObject.UsualObject:
                    return SerializeUsualObject(obj, type, path);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStructuralObject), kindOfStructuralObject, "5A89589A-C1E1-4133-BFAE-9BE1FA882427");
            }
        }

        private SerializedValue SerializeKeyWorldComponent(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.IsPreregistered);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalClassCard()
            {
                Header = serializedValue,
                Path = path
            };

            var cardFieldList = new List<(string, int, SerializedValue)>();

            var fieldsWithSettings = GetFields(type)
                .Where(f => f.Field.IsDefined(typeof(SettingsMemberAttribute), false))
                .ToList();

#if DEBUG
            //_logger.Info($"fieldsWithSettings.Count = {fieldsWithSettings.Count}");
#endif

            foreach(var item in fieldsWithSettings)
            {
#if DEBUG
                //_logger.Info($"field.Name = {field.Name}");
#endif

                ProcessFieldInfo(item.Field, item.TypeId, obj, path, cardFieldList);
            }

            var fields = GetFields(type)
                //.Where(f => f.IsDefined(typeof(SettingsMemberAttribute), false))
                .ToList();

#if DEBUG
            //_logger.Info($"fields.Count = {fields.Count}");
#endif

            foreach(var item in fields)
            {
                var field = item.Field;

#if DEBUG
                //_logger.Info($"field.Name = {field.Name}");
#endif

                if (field.IsDefined(typeof(SerializedMemberAttribute), false))
                {
                    continue;
                }

                if (field.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

#if DEBUG
                TmpCheckProcessedMembersOfTypes("ADC70B21-5098-4EC0-A43B-C118D67A72A9", type, field.Name);
#endif

                ProcessFieldInfo(field, item.TypeId, obj, path, cardFieldList);
            }

            card.Fields = cardFieldList;

            var properties = GetProperties(type);

#if DEBUG
            //_logger.Info($"properties.Length = {properties.Count()}");
#endif

            if(properties.Any())
            {
                foreach (var item in properties)
                {
#if DEBUG
                    //_logger.Info($"type.FullName = {type.FullName}");
                    //_logger.Info($"property.Name = {property.Name}");
#endif

#if DEBUG
                    TmpCheckProcessedMembersOfTypes("675FF756-50FE-4073-9B08-7AAA916ACCD9", type, item.Prop.Name);
#endif
                }

                throw new NotImplementedException("CFD2C27A-2E72-40D7-A365-3A6BFE4467CF");
            }

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            //throw new NotImplementedException("C90EDF11-865C-4725-ABA4-A803814DC014");

            return serializedValue;
        }

        private SerializedValue SerializeSettings(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
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

            var cardPropertyList = new List<(string, int, SerializedValue)>();

            var properties = GetProperties(type);

#if DEBUG
            //_logger.Info($"properties.Count() = {properties.Count()}");
#endif

            foreach (var item in properties)
            {
                var property = item.Prop;

#if DEBUG
                //_logger.Info($"property.Name = {property.Name}");
#endif

                if (property.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

#if DEBUG
                TmpCheckProcessedMembersOfTypes("85FEC95E-9478-4037-BB84-7F1C6328872B", type, property.Name);
#endif

                ProcessPropertyInfo(property, item.TypeId, obj, path, cardPropertyList);
            }

            card.Properties = cardPropertyList;

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            //throw new NotImplementedException("C9EBF62E-2FB3-4241-A9BB-E70A5D6A0774");

            return serializedValue;
        }

        private SerializedValue SerializeUsualObject(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
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

            var cardFieldList = new List<(string, int, SerializedValue)>();

            foreach (var item in fields)
            {
                var field = item.Field;

#if DEBUG
                //_logger.Info($"field.Name = {field.Name}");
#endif

                if (field.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

#if DEBUG
                TmpCheckProcessedMembersOfTypes("E09901B9-7B79-4DD3-9BD3-1F89B1AFC600", type, field.Name);
#endif

                ProcessFieldInfo(field, item.TypeId, obj, path, cardFieldList);
            }

            card.Fields = cardFieldList;

            var cardPropertyList = new List<(string, int, SerializedValue)>();

            var propertyInfos = GetProperties(type);

#if DEBUG
            //_logger.Info($"propertyInfos.Count() = {propertyInfos.Count()}");
#endif

            foreach (var item in propertyInfos)
            {
                var property = item.Prop;

#if DEBUG
                //_logger.Info($"property.Name = {property.Name}");
#endif

                if (property.IsDefined(typeof(SystemNoSerializedMemberAttribute), false))
                {
                    continue;
                }

#if DEBUG
                TmpCheckProcessedMembersOfTypes("8CF41E0D-1BEB-4BAF-8D3A-6107D6B788D7", type, property.Name);
#endif

                ProcessPropertyInfo(property, item.TypeId, obj, path, cardPropertyList);
            }

            card.Properties = cardPropertyList;

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            //throw new NotImplementedException("C871A015-857B-4C95-B5AA-A4DCAB10EB43");

            return serializedValue;
        }

        private void ProcessFieldInfo(FieldInfo field, int typeId, object obj, string path, List<(string, int, SerializedValue)> cardFieldList)
        {
            var fieldValue = field.GetValue(obj);

#if DEBUG
            //_logger.Info($"fieldValue = {fieldValue}");
#endif

            var fieldPath = $"{path}/{field.Name}";

#if DEBUG
            //_logger.Info($"fieldPath = {fieldPath}");
#endif

            var fieldSerializedValue = SerializeValue(fieldValue, fieldPath);

#if DEBUG
            //_logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

            cardFieldList.Add((field.Name, typeId, fieldSerializedValue));
        }

        private void ProcessPropertyInfo(PropertyInfo property, int typeId, object obj, string path, List<(string, int, SerializedValue)> cardPropertyList)
        {
            var propertyValue = property.GetValue(obj);

#if DEBUG
            //_logger.Info($"propertyValue = {propertyValue}");
#endif

            var serializeValueMode = SerializeValueMode.General;

            if (property.IsDefined(typeof(MemberWithExternalValueAttribute), true))
            {
                serializeValueMode = SerializeValueMode.ExternalValue;
            }

            var propertyPath = $"{path}/{property.Name}";

#if DEBUG
            //_logger.Info($"propertyPath = {propertyPath}");
#endif

            var propertySerializedValue = SerializeValue(propertyValue, propertyPath, serializeValueMode);

#if DEBUG
            //_logger.Info($"propertySerializedValue = {propertySerializedValue}");
#endif

            cardPropertyList.Add((property.Name, typeId, propertySerializedValue));
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeManualResetEvent(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var autoResetEvent = (ManualResetEvent)obj;

            var isSet = autoResetEvent.WaitOne(0);

#if DEBUG
            //_logger.Info($"isSet = {isSet}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalManualResetEventClassCard()
            {
                Header = serializedValue,
                Path = path,
                IsSet = isSet
            };

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);


            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj, Type type)
        {
#if DEBUG
            TmpCheckProcessedTypes("D6559AD4-8808-4D7A-95C8-EAB973C4ADA6", type);
#endif

            throw new NotImplementedException("C6866BBD-6E0A-46CD-BBCA-2D9B66381B2B");
        }

#if DEBUG
        /// <inheritdoc/>
        protected override List<string> _tmpProcessedTypes { get; set; } = new List<string>()
        {
            "TestSandbox.SerializationToImage.TstWorldContext",
            "SymOntoClay.UnityAsset.Core.World.WorldCore",
            "SymOntoClay.UnityAsset.Core.WorldSettings",
            "SymOntoClay.Core.ThreadingSettings",
            "SymOntoClay.Threading.CustomThreadPoolSettings",
            "SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCImplementation",
            "SymOntoClay.UnityAsset.Core.HumanoidNPCSettings"
        };

        /// <inheritdoc/>
        protected override Dictionary<string, List<string>> _tmpProcessedMembersOfTypes { get; set; } = new Dictionary<string, List<string>>();

        private void InitTmpProcessedMembersOfTypes()
        {
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.WorldSettings"] = new List<string>()
            {
                "LibsDirs",
                "ImagesRootDir",
                "DictionariesDirs",
                "BuiltInStandardLibraryDir",
                "TmpDir",
                "Monitor",
                "HostFile",
                "InvokerInMainThread",
                "SoundBus",
                "NLPConverterProvider",
                "StandardFactsBuilder",
                "EnableAutoloadingConvertors",
                "CancellationContext",
                "WorldThreadingSettings",
                "HumanoidNpcDefaultThreadingSettings",
                "PlayerDefaultThreadingSettings",
                "GameObjectDefaultThreadingSettings",
                "PlaceDefaultThreadingSettings",
                "HtnExecutionDefaultSettings"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.ThreadingSettings"] = new List<string>()
            { 
                "CodeExecution",
                "AsyncEvents",
                "GarbageCollection"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Threading.CustomThreadPoolSettings"] = new List<string>()
            {
                "MinThreadsCount",
                "MaxThreadsCount"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.World.WorldCore"] = new List<string>()
            { 
                "_settings",
                "_lockObj",
                "_state",
                "_serializedWorldComponents",
                "_platformTypesConverters"//tmp
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.HumanoidNPCSettings"] = new List<string>()
            { 
                "LogicFile",
                "PlatformSupport",
                "VisionProvider",
                "Categories",
                "EnableCategories",
                "HtnExecutionSettings",
                "_id",
                "_idForFacts",
                "HostListener",
                "InstanceId",
                "AllowPublicPosition",
                "UseStaticPosition",
                "ThreadingSettings"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCImplementation"] = new List<string>()
            {
                "_settings"
            };
        }
#endif
    }
}
