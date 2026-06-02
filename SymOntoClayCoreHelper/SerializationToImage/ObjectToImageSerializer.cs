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
        
        public ObjectToImageSerializer(ISerializedObjectsPool serializedObjectsPool, ISerializedTypesPool serializedTypesPool, IStructuralContext structuralContext, IDataCardWriter dataCardWriter)
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
        protected override SerializedValue SerializeReflectionType(object obj, Type type)
        {
#if DEBUG
            _logger.Info($"obj?.GetType()?.FullName = {obj?.GetType()?.FullName}");
            _logger.Info($"type.FullName = {type.FullName}");
#endif

            var typeObj = (Type)obj;

#if DEBUG
            _logger.Info($"typeObj.FullName = {typeObj.FullName}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ReflectionTypeCard()
            {
                Header = serializedValue,
                OriginalTypeName = typeObj.FullName
            };

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
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

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
#endif

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
                //_logger.Info($"item.Key?.GetType()?.FullName = {item.Key?.GetType()?.FullName}");
                _logger.Info($"item.Value = {item.Value}");
#endif

                var keySerializedValue = SerializeValue(item.Key, string.Empty);

#if DEBUG
                _logger.Info($"keySerializedValue = {keySerializedValue}");
#endif

                var valueSerializedValue = SerializeValue(item.Value, string.Empty);

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
                .Where(f => f.Field.IsDefined(typeof(SerializedMemberAttribute), false))
                .ToList();

#if DEBUG
            _logger.Info($"fieldsWithSerializedMembers.Count = {fieldsWithSerializedMembers.Count}");
#endif

            foreach (var item in fieldsWithSerializedMembers)
            {
                var field = item.Field;

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
                .Where(f => f.Field.IsDefined(typeof(SerializedMemberWithChildrenAttribute), false))
                .ToList();

#if DEBUG
            _logger.Info($"fieldsWithChildren.Count = {fieldsWithChildren.Count}");
#endif

            foreach(var item in fieldsWithChildren)
            {
                var field = item.Field;

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

            var cardFieldList = new List<(string, int, SerializedValue)>();

            foreach (var item in fields)
            {
                var field = item.Field;

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

                ProcessFieldInfo(field, item.TypeId, obj, cardFieldList);
            }

            card.Fields = cardFieldList;

            var cardPropertyList = new List<(string, int, SerializedValue)>();

            var propertyInfos = GetProperties(type);

#if DEBUG
            _logger.Info($"propertyInfos.Count() = {propertyInfos.Count()}");
#endif

            foreach (var item in propertyInfos)
            {
                var property = item.Prop;

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

                ProcessPropertyInfo(property, item.TypeId, obj, cardPropertyList);
            }

            card.Properties = cardPropertyList;

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        private void ProcessFieldInfo(FieldInfo field, int typeId, object obj, List<(string, int, SerializedValue)> cardFieldList)
        {
            var fieldValue = field.GetValue(obj);

#if DEBUG
            _logger.Info($"fieldValue = {fieldValue}");
#endif

            var fieldSerializedValue = SerializeValue(fieldValue, string.Empty);

#if DEBUG
            _logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

            cardFieldList.Add((field.Name, typeId, fieldSerializedValue));
        }

        private void ProcessPropertyInfo(PropertyInfo property, int typeId, object obj, List<(string, int, SerializedValue)> cardPropertyList)
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

            cardPropertyList.Add((property.Name, typeId, propertySerializedValue));
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
            "SymOntoClay.Core.Internal.Storage.LogicalStoraging.ConsolidatedPublicFactsLogicalStorage",
            "SymOntoClay.Core.Internal.Storage.ConsolidatedPublicFactsStorage",
            "SymOntoClay.Core.Internal.Storage.RealStorage",
            "SymOntoClay.Core.Internal.Htn.HtnExecutorComponent",
            "SymOntoClay.Core.Internal.EngineContext",
            "SymOntoClay.Core.Internal.CodeExecution.CodeExecutorComponent",
            "SymOntoClay.Core.Internal.Services.CodeFrameService",
            "SymOntoClay.Core.Internal.DataResolvers.BaseResolver",
            "SymOntoClay.Core.Internal.DataResolvers.InheritanceResolver",
            "SymOntoClay.Core.Internal.DataResolvers.ResolverOptions",
            "SymOntoClay.Core.Internal.DataResolvers.LogicalValueLinearResolver",
            "SymOntoClay.Core.Internal.DataResolvers.FuzzyLogicResolver",
            "SymOntoClay.Core.Internal.DataResolvers.ToSystemBoolResolver",
            "SymOntoClay.Core.Internal.DataResolvers.NumberValueLinearResolver",
            "SymOntoClay.Core.Internal.DataResolvers.SynonymsResolver",
            "SymOntoClay.Core.Internal.DataResolvers.OperatorsResolver",
            "SymOntoClay.Core.Internal.DataResolvers.MethodsResolver",
            "SymOntoClay.Core.Internal.Converters.TypeConverter",
            "SymOntoClay.Core.Internal.DataResolvers.LogicalSearchResolver",
            "SymOntoClay.Core.Internal.DataResolvers.VarsResolver",
            "SymOntoClay.Core.Internal.DataResolvers.PropertiesResolver",
            "SymOntoClay.Core.Internal.DataResolvers.LogicalSearchVarResultsItemInvertor",
            "SymOntoClay.Core.Internal.Converters.TypeFitCheckingResult",
            "SymOntoClay.Core.Internal.CodeExecution.LocalCodeExecutionContext",
            "SymOntoClay.Core.Internal.StandardLibrary.StandardLibraryLoader",
            "SymOntoClay.UnityAsset.Core.Internal.HostSupport.HostSupportComponent",
            "SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCGameComponent",
            "SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCGameComponentContext",
            "SymOntoClay.UnityAsset.Core.Internal.SoundPerception.SoundPublisherComponent",
            "SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCGameComponentSerializedContext",
            "SymOntoClay.Core.Engine",
            "SymOntoClay.UnityAsset.Core.Internal.ConditionalEntityHostSupport.ConditionalEntityHostSupportComponent",
            "SymOntoClay.UnityAsset.Core.Internal.SoundPerception.SoundReceiverComponent",
            "SymOntoClay.Core.Internal.Storage.InheritanceStoraging.ConsolidatedPublicFactsInheritanceStorage",
            "SymOntoClay.Core.Internal.Storage.TriggersStoraging.EmptyTriggersStorage",
            "SymOntoClay.Core.Internal.Storage.VarStoraging.EmptyVarStorage",
            "SymOntoClay.Core.Internal.Storage.StatesStoraging.EmptyStatesStorage",
            "SymOntoClay.Core.Internal.Storage.RelationStoraging.EmptyRelationsStorage",
            "SymOntoClay.Core.Internal.Storage.MethodsStoraging.EmptyMethodsStorage",
            "SymOntoClay.Core.Internal.Storage.ConstructorsStoraging.EmptyConstructorsStorage",
            "SymOntoClay.Core.Internal.Storage.ActionsStoraging.EmptyActionsStorage",
            "SymOntoClay.Core.Internal.Storage.SynonymsStoraging.EmptySynonymsStorage",
            "SymOntoClay.Core.Internal.Storage.OperatorsStoraging.EmptyOperatorsStorage",
            "SymOntoClay.Core.Internal.Storage.ChannelsStoraging.EmptyChannelsStorage",
            "SymOntoClay.Core.Internal.Storage.MetadataStoraging.EmptyMetadataStorage",
            "SymOntoClay.Core.Internal.Storage.FuzzyLogic.EmptyFuzzyLogicStorage",
            "SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging.EmptyIdleActionItemsStorage",
            "SymOntoClay.Core.Internal.Storage.TasksStoraging.EmptyTasksStorage",
            "SymOntoClay.Core.Internal.Storage.PropertyStoraging.EmptyPropertyStorage",
            "SymOntoClay.UnityAsset.Core.Internal.EndPoints.EndpointsRegistry",
            "SymOntoClay.UnityAsset.Core.Internal.EndPoints.EndPointsResolver",
            "SymOntoClay.UnityAsset.Core.Internal.TypesConverters.PlatformTypesConvertersRegistry",
            "SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.EntityAndStrongIdentifierValueConverter",
            "SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.NavTargetAndStrongIdentifierValueConverter",
            "SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Vector3AndStrongIdentifierValueConverter",
            "SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.FloatAndNumberValueConverter",
            "SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Vector3AndIEntityConverter",
            "SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Vector3AndWayPointValueConverter",
            "SymOntoClay.UnityAsset.Core.Internal.EndPoints.EndPointActivator",
            "SymOntoClay.NLP.NLPConverterFactory",
            "SymOntoClay.NLP.SerializationData.NLPConverterFactorySerializationData",
            "SymOntoClay.Core.Internal.Storage.Factories.StorageFactories"
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.LogicalStoraging.ConsolidatedPublicFactsLogicalStorage"] = new List<string>() 
            { 
                "_lockObj",
                "_mainStorageContext",
                "_parent",
                "_logicalStorages",
                "_rejectedFacts",
                "_processedOnAddingFacts",
                "_onAddingFactLockObj",
                "_enableOnAddingFactEvent",
                "_fuzzyLogicResolver",
                "_localCodeExecutionContext",
                "_kind",
                "_onChangedHandlersLockObj",
                "_onChangedHandlers",
                "_onChangedWithKeysHandlersLockObj",
                "_onChangedWithKeysHandlers",
                "_onAddingFactHandlerLockObj",
                "_onAddingFactHandlers",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.ConsolidatedPublicFactsStorage"] = new List<string>()
            {
                "_lockObj",
                "_storages",
                "_logicalStorage",
                "_inheritanceStorage",
                "_triggersStorage",
                "_varStorage",
                "_statesStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_kind",
                "_state",
                "_stateLockObj",
                "_logger",
                "CodeItemsStoragesList"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.RealStorage"] = new List<string>() 
            {
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_kind"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Htn.HtnExecutorComponent"] = new List<string>() 
            {
                "_context",
                "_activeObject",
                "_htnPlanner",
                "_compiler",
                "_executionState",
                "_plan",
                "_threadExecutor",
                "_planExecutionIterationsMaxCount",
                "_runPlanExecutionIterations"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.EngineContext"] = new List<string>() 
            { 
                "_state",
                "_stateLockObj",
                "_logger",
                "CodeExecutor",
                "HtnExecutor",
                "StandardLibraryLoader",
                "HostSupport",
                "HostListener",
                "ConditionalEntityHostSupport",
                "SoundPublisherProvider",
                "NLPConverterFactory",
                "StorageFactories",
                "CodeExecutionThreadPool",
                "TriggersThreadPool"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.CodeExecutorComponent"] = new List<string>()
            {
                "_context",
                "_codeFrameService",
                "_operatorsResolver",
                "_methodsResolver",
                "_numberValueLinearResolver",
                "_globalExecutionContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Services.CodeFrameService"] = new List<string>() 
            { 
                "_context",
                "_baseResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.BaseResolver"] = new List<string>() 
            {
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.InheritanceResolver"] = new List<string>() 
            {
                "DefaultOptions",
                "_logicalValueLinearResolver",
                "_synonymsResolver",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.ResolverOptions"] = new List<string>() 
            {
                "AddSelf",
                "AddTopType",
                "OnlyDirectInheritance",
                "JustDistinct",
                "SkipRealSearching"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.LogicalValueLinearResolver"] = new List<string>() 
            { 
                "_fuzzyLogicResolver",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.FuzzyLogicResolver"] = new List<string>() 
            {
                "_toSystemBoolResolver",
                "_numberValueLinearResolver",
                "_synonymsResolver",
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.ToSystemBoolResolver"] = new List<string>() 
            { 
                "TruthThreshold",
                "NullValueEquvivalent",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.NumberValueLinearResolver"] = new List<string>()
            { 
                "_fuzzyLogicResolver",
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.SynonymsResolver"] = new List<string>() 
            { 
                "DefaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.OperatorsResolver"] = new List<string>() 
            {
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.MethodsResolver"] = new List<string>() 
            { 
                "_defaultOptions",
                "_emptyParametersRankMatrix",
                "_synonymsResolver",
                "_typeConverter",
                "_fuzzyTypeName",
                "_numberTypeName",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Converters.TypeConverter"] = new List<string>()
            {
                "_context",
                "_inheritanceResolver",
                "_logicalSearchResolver",
                "_fuzzyLogicResolver",
                "_anyTypeName",
                "_booleanTypeName",
                "_fuzzyTypeName",
                "_numberTypeName",
                "_emptyTypesList",
                "_needConversionToBooleanTypeFitCheckingResult",
                "_defaultOptions",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.LogicalSearchResolver"] = new List<string>() 
            {
                "_fuzzyLogicResolver",
                "_numberValueLinearResolver",
                "_varsResolver",
                "_synonymsResolver",
                "_propertiesResolver",
                "_logicalSearchVarResultsItemInvertor",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.VarsResolver"] = new List<string>()
            { 
                "_anyTypeName",
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.PropertiesResolver"] = new List<string>() 
            {
                "_codeExecutorComponent",
                "_standardCoreFactsBuilder",
                "_logicalSearchResolver",
                "_targetLogicalVarName",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger",
                "DefaultOptions"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.LogicalSearchVarResultsItemInvertor"] = new List<string>() 
            {
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Converters.TypeFitCheckingResult"] = new List<string>() 
            { 
                "KindOfResult",
                "SuggestedType"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.LocalCodeExecutionContext"] = new List<string>() 
            {
                "Parent",
                "UseParentInResolving",
                "IsIsolated",
                "Holder",
                "Storage",
                "Instance",
                "Owner",
                "OwnerStorage",
                "Kind",
                "KindOfAddFactResult",
                "AddedRuleInstance"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.StandardLibrary.StandardLibraryLoader"] = new List<string>() 
            { 
                "_context",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.HostSupport.HostSupportComponent"] = new List<string>() 
            {
                "_invokerInMainThread",
                "_platformSupport",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCGameComponent"] = new List<string>() 
            {
                "_settings",
                "_internalContext",
                "_internalSerializedContext",
                "_allowPublicPosition",
                "_hostSupport",
                "_soundPublisher",
                "_endpointsRegistries",
                "_hostListener",
                "_hostEndpointsRegistry",
                "_endPointsResolver",
                "_endPointActivator",
                "_internalManualControlledObjectsList",
                "_internalManualControlledObjectsDict",
                "_endpointsRegistryForManualControlledObjectsDict",
                "_manualControlLockObj",
                "_worldContext",
                "_logger",
                "_monitorNode",
                "_invokerInMainThread",
                "_instanceId",
                "_cancellationTokenSourceContext",
                "_linkedCancellationTokenSourceContext",
                "_standardFactsBuilder",
                "_idForFacts",
                "_id",
                "_componentState",
                "_stateLockObj",
                "AsyncEventsThreadPool"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCGameComponentContext"] = new List<string>() 
            { 
                "IdForFacts",
                "SelfInstanceId",
                "TmpDir",
                "CancellationContext",
                "HostSupportComponent",
                "AsyncEventsThreadPool",
                "SoundPublisherComponent"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.SoundPerception.SoundPublisherComponent"] = new List<string>() 
            { 
                "_soundBus",
                "_hostSupport",
                "_instanceId",
                "_idForFacts",
                "_standardFactsBuilder",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC.HumanoidNPCGameComponentSerializedContext"] = new List<string>() 
            { 
                "_isDisposed",
                "_lockObj",
                "VisionComponent",
                "CoreEngine",
                "ConditionalEntityHostSupportComponent",
                "SoundReceiverComponent",
                "BackpackStorage"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Engine"] = new List<string>() 
            { 
                "_context",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.ConditionalEntityHostSupport.ConditionalEntityHostSupportComponent"] = new List<string>() 
            {
                "_worldContext",
                "_visionComponent",
                "_hostSupport",
                "_instanceId",
                "_id",
                "_idForFacts",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.SoundPerception.SoundReceiverComponent"] = new List<string>() 
            {
                "_internalContext",
                "_internalSerializedContext",
                "_soundBus",
                "_hostSupport",
                "_coreEngine",
                "_standardFactsBuilder",
                "_instanceId",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.InheritanceStoraging.ConsolidatedPublicFactsInheritanceStorage"] = new List<string>() 
            {
                "_lockObj",
                "_parent",
                "_inheritanceStorages",
                "_kind",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.TriggersStoraging.EmptyTriggersStorage"] = new List<string>() 
            { 
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.VarStoraging.EmptyVarStorage"] = new List<string>() 
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.StatesStoraging.EmptyStatesStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.RelationStoraging.EmptyRelationsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.MethodsStoraging.EmptyMethodsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.ConstructorsStoraging.EmptyConstructorsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.ActionsStoraging.EmptyActionsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.SynonymsStoraging.EmptySynonymsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.OperatorsStoraging.EmptyOperatorsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.ChannelsStoraging.EmptyChannelsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.MetadataStoraging.EmptyMetadataStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.FuzzyLogic.EmptyFuzzyLogicStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging.EmptyIdleActionItemsStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.TasksStoraging.EmptyTasksStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.PropertyStoraging.EmptyPropertyStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.EndPoints.EndpointsRegistry"] = new List<string>() 
            { 
                "_lockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.EndPoints.EndPointsResolver"] = new List<string>() 
            { 
                "_platformTypesConvertorsRegistry",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.TypesConverters.PlatformTypesConvertersRegistry"] = new List<string>() 
            { 
                "_lockObj",
                "_convertersDict",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.EntityAndStrongIdentifierValueConverter"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.NavTargetAndStrongIdentifierValueConverter"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Vector3AndStrongIdentifierValueConverter"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.FloatAndNumberValueConverter"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Vector3AndIEntityConverter"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Vector3AndWayPointValueConverter"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.EndPoints.EndPointActivator"] = new List<string>()
            {
                "_platformTypesConvertorsRegistry",
                "_invokingInMainThread",
                "_threadPool",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.NLP.NLPConverterFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.NLP.SerializationData.NLPConverterFactorySerializationData"] = new List<string>() 
            { 
                "Provider" 
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.StorageFactories"] = new List<string>() 
            {
                "_appInstanceStorageFactory" 
            };
        }
#endif
    }
}
