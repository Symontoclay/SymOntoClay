using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

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
        protected override SerializedValue SerializeExternalValue(object obj)
        {
            return _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.ExternalValue);
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

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj, Type type, string path)
        {
            throw new NotImplementedException("C577B505-79EB-4EB0-81D6-CEE7E181C31D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
            throw new NotImplementedException("C3BE6016-0DA1-4238-BB7E-C12668369925");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new DictionaryCard()
            {
                Header = serializedValue
            };

            var dictionary = (IDictionary)obj;

            var items = new List<KeyValuePair<SerializedValue, SerializedValue>>();

            foreach (DictionaryEntry item in dictionary) 
            {
#if DEBUG
                _logger.Info($"item.Key = {item.Key}");
                _logger.Info($"item.Value = {item.Value}");
#endif

                var keySerializedValue = _serializedObjectsPool.GetOrRegSerializedValue(item.Key, SerializedObjectsPoolMode.General);

#if DEBUG
                _logger.Info($"keySerializedValue = {keySerializedValue}");
#endif

                var valueSerializedValue = _serializedObjectsPool.GetOrRegSerializedValue(item.Value, SerializedObjectsPoolMode.General);

#if DEBUG
                _logger.Info($"valueSerializedValue = {valueSerializedValue}");
#endif

                items.Add(new KeyValuePair<SerializedValue, SerializedValue>(keySerializedValue, valueSerializedValue));
            }

            card.Items = items;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj, Type type, string path)
        {
#if DEBUG
            TmpCheckProcessedTypes("1A078DE9-D217-43DD-AC25-642895615864", type);
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

#if DEBUG
                TmpCheckProcessedMembersOfTypes("163EDDE8-6AFC-4E7F-8BEB-3EA8A0517FDD", type, field.Name);
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

#if DEBUG
                TmpCheckProcessedMembersOfTypes("1084D544-1416-4C3D-8B41-4A1E4251E9BF", type, field.Name);
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

#if DEBUG
                TmpCheckProcessedMembersOfTypes("EE52FE77-A62A-49E4-AF17-A18DEFB8F967", type, field.Name);
#endif

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

#if DEBUG
                TmpCheckProcessedMembersOfTypes("A1F98420-EC02-477B-A384-863381A86F5F", type, property.Name);
#endif

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

            var serializeValueMode = GetSerializeValueMode(property);

            /*if (property.IsDefined(typeof(MemberWithExternalValueAttribute), true))
            {
                serializeValueMode = SerializeValueMode.ExternalValue;
            }*/

            var propertySerializedValue = SerializeValue(propertyValue, string.Empty, serializeValueMode);

#if DEBUG
            _logger.Info($"propertySerializedValue = {propertySerializedValue}");
#endif

            cardPropertyDict[property.Name] = propertySerializedValue;
        }

        private SerializeValueMode GetSerializeValueMode(MemberInfo member)
        {
            var attr = member.GetCustomAttribute<MemberWithExternalValueAttribute>();

            if(attr == null)
            {
                return SerializeValueMode.General;
            }

            if(attr.KindOfStructuralContext == KindOfStructuralContext.All)
            {
                return SerializeValueMode.ExternalValue;
            }

            if(_structuralContext.Kind == attr.KindOfStructuralContext)
            {
                return SerializeValueMode.ExternalValue;
            }

            return SerializeValueMode.General;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeManualResetEvent(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            _logger.Info($"path = {path}");
#endif

            var autoResetEvent = (ManualResetEvent)obj;

            var isSet = autoResetEvent.WaitOne(0);

#if DEBUG
            _logger.Info($"isSet = {isSet}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

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

            _dataCardWriter.Write(card);


            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj, Type type)
        {
#if DEBUG
            TmpCheckProcessedTypes("6D0075E7-92BE-4D55-9E4A-7E56083FB788", type);
#endif

            throw new NotImplementedException("C741439E-BC15-4F4C-8F0A-C775975A3863");
        }

#if DEBUG
        /// <inheritdoc/>
        protected override List<string> _tmpProcessedTypes { get; set; } = new List<string>()
        {
            "SymOntoClay.UnityAsset.Core.World.WorldCore",
            "SymOntoClay.UnityAsset.Core.Internal.WorldContext",
            "SymOntoClay.UnityAsset.Core.Internal.SerializedWorldContext",
            "SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache.LogicQueryParseAndCache",
            "SymOntoClay.Core.Internal.BaseCoreContext",
            "SymOntoClay.Monitor.Internal.MonitorNode",
            "SymOntoClay.Monitor.Common.SerializationData.MonitorNodeSerializationData",
            "SymOntoClay.UnityAsset.Core.Internal.DateAndTime.DateTimeProvider",
            "SymOntoClay.ActiveObject.Threads.AsyncActivePeriodicObject",
            "SymOntoClay.ActiveObject.Threads.ActiveObjectContext",
            "SymOntoClay.UnityAsset.Core.Internal.Threads.ThreadsCoreComponent",
            "SymOntoClay.ActiveObject.Threads.ActiveObjectCommonContext",
            "SymOntoClay.Common.Cancellation.CancellationLinkedTokenSourceContext",
            "SymOntoClay.Common.Cancellation.CancellationTokenSourceContext",
            "SymOntoClay.Threading.CustomThreadPool",
            "SymOntoClay.ActiveObject.Pointers.ThreadTaskPointer",
            "SymOntoClay.ActiveObject.EventsCollections.OnCompletedActiveObjectHandlersCollection",
            "SymOntoClay.Core.Internal.Parsing.Parser",
            "SymOntoClay.UnityAsset.Core.Internal.ModulesStorage.ModulesStorageComponent",
            "SymOntoClay.Core.ModulesStorage",
            "SymOntoClay.Core.Internal.Serialization.ProjectLoader",
            "SymOntoClay.Core.Internal.MainStorageContext",
            "SymOntoClay.Core.Internal.CodeModel.StrongIdentifierValue",
            "SymOntoClay.Core.Internal.Storage.StorageComponent",
            "SymOntoClay.Core.Internal.Storage.StorageComponentSettings",
            "SymOntoClay.Core.Internal.Storage.WorldStorage",
            "SymOntoClay.ActiveObject.Functors.SerializationAnchor",
            "SymOntoClay.Core.Internal.Storage.RealStorageContext",
            "SymOntoClay.Core.Internal.Storage.LogicalStoraging.LogicalStorage",
            "SymOntoClay.Core.Internal.CodeModel.RuleInstance",
            "SymOntoClay.Core.Internal.Storage.LogicalStoraging.CommonPersistIndexedLogicalData",
            "SymOntoClay.Monitor.NLog.MonitorLoggerNLogImplementation",
            "SymOntoClay.Core.Internal.CodeModel.LogicalQueryNode",
            "SymOntoClay.Core.Internal.CodeModel.PrimaryRulePart",
            "SymOntoClay.Core.Internal.CodeModel.LogicalValue",
            "SymOntoClay.Core.Internal.IndexedData.QueryExecutingCardAboutKnownInfo",
            "SymOntoClay.Core.Internal.Storage.LogicalStoraging.ConsolidatedPublicFactsLogicalStorage"
        };

        /// <inheritdoc/>
        protected override Dictionary<string, List<string>> _tmpProcessedMembersOfTypes { get; set; } = new Dictionary<string, List<string>>();

        private void InitTmpProcessedMembersOfTypes()
        {
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.World.WorldCore"] = new List<string>()
            {
                "_context"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.WorldContext"] = new List<string>()
            {
                "_isInitialized",
                "_settings",
                "_tmpDir",
                "_serializedWorldContext"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.SerializedWorldContext"] = new List<string>()
            { 
                "_coreContext",
                "_worldComponentsList",
                "_worldComponentsListLockObj"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.LogicQueryParsingAndCache.LogicQueryParseAndCache"] = new List<string>()
            { 
                "_context",
                "_parser",
                "_cache",
                "_coreContext",
                "_logger",
                "_componentState",
                "_stateLockObj"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.BaseCoreContext"] = new List<string>()
            {
                "MonitorNode",
                "Compiler",
                "DateTimeProvider",
                "StandardFactsBuilder",
                "AsyncEventsThreadPool",
                "GarbageCollectionThreadPool",
                "CancellationTokenSourceContext",
                "LinkedCancellationTokenSourceContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Monitor.Common.SerializationData.MonitorNodeSerializationData"] = new List<string>()
            {
                "Parent",
                "NodeId"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.DateAndTime.DateTimeProvider"] = new List<string>()
            {
                "_lockObj",
                "_activeObject",
                "_ticks",
                "_millisecondsTimeout",
                "_ulongMillisecondsTimeout",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Threads.AsyncActivePeriodicObject"] = new List<string>()
            {
                "_context",
                "_threadPool",
                "_cancellationContext",
                "_logger",
                "_lockObj",
                "_isWaited",
                "_isExited",
                "_task",
                "_onCompletedHandlersCollection",
                "_isDisposed",
                "ObjectWithPeriodicMethod"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Threads.ActiveObjectContext"] = new List<string>()
            {
                "_commonContext",
                "_cancellationContext",
                "_lockObj",
                "_periodicChildren",
                "_onceChildren",
                "_isDisposed"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.Threads.ThreadsCoreComponent"] = new List<string>()
            {
                "_lockObj",
                "_isLocked",
                "_commonActiveContext",
                "_coreContext",
                "_logger",
                "_componentState",
                "_stateLockObj"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Threads.ActiveObjectCommonContext"] = new List<string>()
            {
                "_autoResetEvent",
                "_isNeedWating"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Common.Cancellation.CancellationLinkedTokenSourceContext"] = new List<string>()
            {
                "_cancellationContext1",
                "_cancellationContext2",
                "_isDisposed",
                "_lockObj"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Common.Cancellation.CancellationTokenSourceContext"] = new List<string>()
            { 
                "_isDisposed",
                "_lockObj"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Threading.CustomThreadPool"] = new List<string>() 
            { 
                "_settings",
                "_maxThreadsCount",
                "_minThreadsCount",
                "_cancellationContext",
                "_needToRun"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Pointers.ThreadTaskPointer"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.EventsCollections.OnCompletedActiveObjectHandlersCollection"] = new List<string>() 
            { 
                "_lockObj",
                "_handlers"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Parsing.Parser"] = new List<string>() 
            { 
                "_context",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.ModulesStorage.ModulesStorageComponent"] = new List<string>() 
            { 
                "_modulesStorage" 
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.ModulesStorage"] = new List<string>() 
            { 
                "_projectLoader",
                "_mainStorageContext",
                "_lockObj"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Serialization.ProjectLoader"] = new List<string>() 
            { 
                "_context",
                "_compiler",
                "_isDeferredImport",
                "_defaultSettingsOfCodeEntity",
                "_globalStorage",
                "_commonNamesStorage"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.MainStorageContext"] = new List<string>()
            { 
                "Id",
                "SelfName",
                "AppFile",
                "ActiveObjectContext",
                "_state",
                "_stateLockObj",
                "_logger",
                "Storage",
                "Parser",
                "DataResolversFactory"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.StrongIdentifierValue"] = new List<string>()
            { 
                "_isNull",
                "IsEmpty",
                "KindOfName",
                "NameValue",
                "NormalizedNameValue",
                "NameWithoutPrefix",
                "IsArray",
                "Capacity",
                "HasInfiniteCapacity",
                "Level",
                "Namespaces",
                "ForResolving",
                "_builtInSuperTypes",
                "_annotationsLockObj",
                "_annotationFacts",
                "_meaningRolesList",
                "_settingsDict",
                "_annotationValueLockObj",
                "_disposingLockObj",
                "_isDisposed",
                "_isDirty",
                "_longConditionalHashCode",
                "_longHashCode",
                "InternalSystemId",
                "WhereSection",
                "Annotations"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.StorageComponent"] = new List<string>()
            { 
                "_settings",
                "_context",
                "_parentStorage",
                "_kindGlobalOfStorage",
                "_logicQueryParseAndCache",
                "_parser",
                "_globalStorage",
                "_publicFactsStorage",
                "_selfFactsStorage",
                "_perceptedFactsStorage",
                "_listenedFactsStorage",
                "_visibleFactsStorage",
                "_worldPublicFactsStorage"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.StorageComponentSettings"] = new List<string>() 
            { 
                "Categories",
                "EnableCategories"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.WorldStorage"] = new List<string>() 
            { 
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_kind",
                "_realStorageContext"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Functors.SerializationAnchor"] = new List<string>() 
            { 
                "_lockObj",
                "_functors"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.RealStorageContext"] = new List<string>() 
            { 
                "_onAddParentStorageHandlersLockObj",
                "_onAddParentStorageHandlers"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.LogicalStoraging.LogicalStorage"] = new List<string>()
            { 
                "_lockObj",
                "_ruleInstancesList",
                "_factsList",
                "_ruleInstancesDict",
                "_ruleInstancesDictByHashCode",
                "_ruleInstancesDictById",
                "_lifeTimeCycleById",
                "_enableAddingRemovingFactLoggingInStorages",
                "_commonPersistIndexedLogicalData",
                "_parentLogicalStoragesList",
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_activeObject",
                "_enableOnAddingFactEvent",
                "_fuzzyLogicResolver",
                "_localCodeExecutionContext",
                "_dateTimeProvider"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.RuleInstance"] = new List<string>() 
            {
                "_builtInSuperTypes",
                "_timeStamp",
                "_commonPersistIndexedLogicalData",
                "_synonymsStorage",
                "_synonymsStorageLockObj",
                "_onNameChangedHandlersLockObj",
                "_onNameChangedHandlers",
                "_name",
                "_holder",
                "_typeOfAccess",
                "_annotationsLockObj",
                "_annotationFacts",
                "_meaningRolesList",
                "_settingsDict",
                "_annotationValueLockObj",
                "_disposingLockObj",
                "_isDisposed",
                "_isDirty",
                "_longConditionalHashCode",
                "_longHashCode",
                "IsSource",
                "IsParameterized",
                "KindOfRuleInstance",
                "PrimaryPart",
                "SecondaryParts",
                "ObligationModality",
                "SelfObligationModality",
                "UsedKeysList",
                "LeavesList",
                "Original",
                "Normalized",
                "LogicalStorages",
                "SymOntoClay.Core.IStorage.DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "IsAnonymous",
                "InheritanceItems",
                "CodeFile",
                "ParentCodeEntity",
                "SubItems",
                "Directives",
                "ActivatingConditions",
                "DeactivatingConditions",
                "IdleActionItems",
                "Priority",
                "ImportsList",
                "InternalSystemId",
                "WhereSection",
                "Annotations"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.LogicalStoraging.CommonPersistIndexedLogicalData"] = new List<string>() 
            {
                "_leafsDict",
                "_logicalQueryNodeEqualityComparer",
                "_logger",
                "IndexedRuleInstancesDict",
                "AdditionalRuleInstancesDict",
                "IndexedRulePartsOfFactsDict",
                "IndexedRulePartsWithOneRelationWithVarsDict",
                "RelationsList"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Monitor.NLog.MonitorLoggerNLogImplementation"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.LogicalQueryNode"] = new List<string>() 
            { 
                "_annotationsLockObj",
                "_annotationFacts",
                "_meaningRolesList",
                "_settingsDict",
                "_annotationValueLockObj",
                "_disposingLockObj",
                "_isDisposed",
                "_isDirty",
                "_longConditionalHashCode",
                "_longHashCode",
                "Kind",
                "KindOfOperator",
                "Name",
                "Left",
                "Right",
                "ParamsList",
                "LinkedVars",
                "Value",
                "FuzzyLogicNonNumericSequenceValue",
                "Fact",
                "IsQuestion",
                "IsNull",
                "CountParams",
                "VarsInfoList",
                "KnownInfoList",
                "RuleInstance",
                "RulePart",
                "TypeOfAccess",
                "Holder",
                "InternalSystemId",
                "WhereSection",
                "Annotations"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.PrimaryRulePart"] = new List<string>()
            {
                "_annotationsLockObj",
                "_annotationFacts",
                "_meaningRolesList",
                "_settingsDict",
                "_annotationValueLockObj",
                "_disposingLockObj",
                "_isDisposed",
                "_isDirty",
                "_longConditionalHashCode",
                "_longHashCode",
                "SecondaryParts",
                "Parent",
                "IsActive",
                "Expression",
                "AliasesDict",
                "HasQuestionVars",
                "HasVars",
                "IsParameterized",
                "TypeOfAccess",
                "Holder",
                "RelationsDict",
                "InternalSystemId",
                "WhereSection",
                "Annotations"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.LogicalValue"] = new List<string>() 
            { 
                "_builtInSuperTypes",
                "_isBoolean",
                "_annotationsLockObj",
                "_annotationFacts",
                "_meaningRolesList",
                "_settingsDict",
                "_annotationValueLockObj",
                "_disposingLockObj",
                "_isDisposed",
                "_isDirty",
                "_longConditionalHashCode",
                "_longHashCode",
                "SystemValue",
                "InternalSystemId",
                "WhereSection",
                "Annotations"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.IndexedData.QueryExecutingCardAboutKnownInfo"] = new List<string>() 
            { 
                "Kind",
                "NameOfVar",
                "Position",
                "Expression",
                "AdditionalExpressions"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.LogicalStoraging.ConsolidatedPublicFactsLogicalStorage"] = new List<string>() { };
        }
#endif
    }
}
