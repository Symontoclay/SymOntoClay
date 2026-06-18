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
            //_logger.Info($"obj?.GetType()?.FullName = {obj?.GetType()?.FullName}");
#endif

            if(obj == null)
            {
                return base.TryGetSerializedValue(obj, out serializedValue);
            }

            var type = obj.GetType();

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            //_logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
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
            //_logger.Info($"obj?.GetType()?.FullName = {obj?.GetType()?.FullName}");
            //_logger.Info($"type.FullName = {type.FullName}");
#endif

            var typeObj = (Type)obj;

#if DEBUG
            //_logger.Info($"typeObj.FullName = {typeObj.FullName}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ReflectionTypeCard()
            {
                Header = serializedValue,
                OriginalTypeName = typeObj.FullName
            };

#if DEBUG
            //_logger.Info($"card = {card}");
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
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ClassCard()
            {
                Header = serializedValue
            };

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeArray(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ArrayCard()
            {
                Header = serializedValue
            };

            var enumerable = (IEnumerable)obj;

            var items = new List<SerializedValue>();

            foreach (var item in enumerable)
            {
#if DEBUG
                //_logger.Info($"item = {item}");
#endif

                var fieldSerializedValue = SerializeValue(item, string.Empty);

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
        protected override SerializedValue SerializeGenericList(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
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
                //_logger.Info($"item = {item}");
#endif

                var fieldSerializedValue = SerializeValue(item, string.Empty);

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
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new StackCard()
            {
                Header = serializedValue
            };

            var enumerable = (IEnumerable)obj;

            var items = new List<SerializedValue>();

            foreach (var item in enumerable)
            {
#if DEBUG
                //_logger.Info($"item = {item}");
#endif

                var fieldSerializedValue = SerializeValue(item, string.Empty);

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
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new QueueCard()
            {
                Header = serializedValue
            };

            var enumerable = (IEnumerable)obj;

            var items = new List<SerializedValue>();

            foreach (var item in enumerable)
            {
#if DEBUG
                //_logger.Info($"item = {item}");
#endif

                var fieldSerializedValue = SerializeValue(item, string.Empty);

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
        protected override SerializedValue SerializeHashSet(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new HashSetCard()
            {
                Header = serializedValue
            };

            var enumerable = (IEnumerable)obj;

            var items = new List<SerializedValue>();

            foreach (var item in enumerable)
            {
#if DEBUG
                //_logger.Info($"item = {item}");
#endif

                var fieldSerializedValue = SerializeValue(item, string.Empty);

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
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
            _visitedObjects.Add(obj);

#if DEBUG
            //_logger.Info($"type.FullName = {type.FullName}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
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
                //_logger.Info($"item.Key = {item.Key}");
                //_logger.Info($"item.Key?.GetType()?.FullName = {item.Key?.GetType()?.FullName}");
                //_logger.Info($"item.Value = {item.Value}");
#endif

                var keySerializedValue = SerializeValue(item.Key, string.Empty);

#if DEBUG
                //_logger.Info($"keySerializedValue = {keySerializedValue}");
#endif

                var valueSerializedValue = SerializeValue(item.Value, string.Empty);

#if DEBUG
                //_logger.Info($"valueSerializedValue = {valueSerializedValue}");
#endif

                items.Add(new KeyValuePair<SerializedValue, SerializedValue>(keySerializedValue, valueSerializedValue));
            }

            card.Items = items;

#if DEBUG
            //_logger.Info($"card = {card}");
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
            //_logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
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
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var fieldsWithSerializedMembers = GetFields(type)
                .Where(f => f.Field.IsDefined(typeof(SerializedMemberAttribute), false))
                .ToList();

#if DEBUG
            //_logger.Info($"fieldsWithSerializedMembers.Count = {fieldsWithSerializedMembers.Count}");
#endif

            foreach (var item in fieldsWithSerializedMembers)
            {
                var field = item.Field;

#if DEBUG
                //_logger.Info($"field.Name = {field.Name}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("163EDDE8-6AFC-4E7F-8BEB-3EA8A0517FDD", type, field.Name);
#endif

                var fieldValue = field.GetValue(obj);

#if DEBUG
                //_logger.Info($"fieldValue = {fieldValue}");
#endif

                var fieldSerializedValue = SerializeValue(fieldValue, path);

#if DEBUG
                //_logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
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
                //_logger.Info($"field.Name = {field.Name}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("1084D544-1416-4C3D-8B41-4A1E4251E9BF", type, field.Name);
#endif

                var fieldValue = field.GetValue(obj);

#if DEBUG
                //_logger.Info($"fieldValue = {fieldValue}");
#endif

                var fieldSerializedValue = SerializeValue(fieldValue, path);

#if DEBUG
                //_logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
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
            //_logger.Info($"serializedValue = {serializedValue}");
#endif
            
            var card = new ClassCardWithSerializationData()
            {
                Header = serializedValue
            };

            var serializationDataFactory = (ISerializationDataFactory)obj;

            var serializationData = serializationDataFactory.GetSerializationData();

#if DEBUG
            //_logger.Info($"serializationData = {serializationData}");
#endif

            var serializedSerializationDataValue = SerializeValue(serializationData, string.Empty);

#if DEBUG
            //_logger.Info($"serializedSerializationDataValue = {serializedSerializationDataValue}");
#endif

            card.SerializationData = serializedSerializationDataValue;

#if DEBUG
            //_logger.Info($"card = {card}");
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
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ClassCard()
            {
                Header = serializedValue
            };

            var fields = GetFields(type);

#if DEBUG
            //_logger.Info($"fields.Count() = {fields.Count()}");
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
                TmpCheckProcessedMembersOfTypes("EE52FE77-A62A-49E4-AF17-A18DEFB8F967", type, field.Name);
#endif

                ProcessFieldInfo(field, item.TypeId, obj, cardFieldList);
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
                TmpCheckProcessedMembersOfTypes("A1F98420-EC02-477B-A384-863381A86F5F", type, property.Name);
#endif

                ProcessPropertyInfo(property, item.TypeId, obj, cardPropertyList);
            }

            card.Properties = cardPropertyList;

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        private void ProcessFieldInfo(FieldInfo field, int typeId, object obj, List<(string, int, SerializedValue)> cardFieldList)
        {
            var fieldValue = field.GetValue(obj);

#if DEBUG
            //_logger.Info($"fieldValue = {fieldValue}");
#endif

            var objMember = new ObjMemberRef(obj, field);

            var fieldSerializedValue = SerializeValue(obj: fieldValue, path: string.Empty, objMember: objMember);

#if DEBUG
            //_logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

            cardFieldList.Add((field.Name, typeId, fieldSerializedValue));
        }

        private void ProcessPropertyInfo(PropertyInfo property, int typeId, object obj, List<(string, int, SerializedValue)> cardPropertyList)
        {
            var propertyValue = property.GetValue(obj);

#if DEBUG
            //_logger.Info($"propertyValue = {propertyValue}");
#endif

            var serializeValueMode = GetSerializeValueMode(property);

            /*if (property.IsDefined(typeof(MemberWithExternalValueAttribute), true))
            {
                serializeValueMode = SerializeValueMode.ExternalValue;
            }*/

            var objMember = new ObjMemberRef(obj, property);

            var propertySerializedValue = SerializeValue(obj: propertyValue, path: string.Empty, serializeValueMode: serializeValueMode, objMember: objMember);

#if DEBUG
            //_logger.Info($"propertySerializedValue = {propertySerializedValue}");
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
        protected override SerializedValue SerializeAction(object obj, Type type, ObjMemberRef objMember)
        {
            if(objMember == null)
            {
                throw new NotImplementedException("E8497C8F-FC96-488E-BCD3-2143B8AF0858");
            }

            var attribute = objMember.GetCustomAttribute<SerializedActionMemberAttribute>(true);

            if (attribute == null)
            {
                throw new NotImplementedException("23B60F49-9DC0-4065-93E7-25CFD9B5C69F");
            }

#if DEBUG
            //_logger.Info($"attribute = {attribute}");
            //_logger.Info($"attribute.KeyParameterName = {attribute.KeyParameterName}");
            //_logger.Info($"attribute.Index = {attribute.Index}");
#endif

            var keyValue = (string)objMember.GetValue(attribute.KeyParameterName);

#if DEBUG
            //_logger.Info($"keyValue = {keyValue}");
#endif

            var serializedValue = _serializedObjectsPool.GetOrRegSerializedValue(obj, SerializedObjectsPoolMode.General);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ActionCard()
            {
                Header = serializedValue,
                KeyValue = keyValue,
                Index = attribute.Index
            };

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
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
            "SymOntoClay.Monitor.Internal.ThreadLogger",
            "SymOntoClay.Monitor.Common.SerializationData.MonitorNodeSerializationData",
            "SymOntoClay.Monitor.Common.SerializationData.ThreadLoggerSerializationData",
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
            "SymOntoClay.Core.Internal.Storage.Factories.StorageFactories",
            "SymOntoClay.Core.Internal.Storage.Factories.AppInstanceStorageFactory",
            "SymOntoClay.Core.Internal.Storage.Factories.ObjectStorageFactory",
            "SymOntoClay.Core.Internal.Storage.Factories.StateStorageFactory",
            "SymOntoClay.Core.Internal.Storage.Factories.ActionStorageFactory",
            "SymOntoClay.Core.Internal.Storage.Factories.RootTaskInstanceStorageFactory",
            "SymOntoClay.Core.Internal.Storage.Factories.StrategicTaskInstanceStorageFactory",
            "SymOntoClay.Core.Internal.Storage.Factories.TacticalTaskInstanceStorageFactory",
            "SymOntoClay.Core.Internal.Storage.Factories.CompoundTaskInstanceStorageFactory",
            "SymOntoClay.Core.StandaloneStorage",
            "SymOntoClay.Core.Internal.Storage.GlobalStorage",
            "SymOntoClay.Core.Internal.Storage.LogicalStoraging.EmptyLogicalStorage",
            "SymOntoClay.Core.Internal.Htn.BuildPlanIterationStorage",
            "SymOntoClay.Core.Internal.Storage.AppInstanceStorage",
            "SymOntoClay.Core.Internal.CodeModel.NumberValue",
            "SymOntoClay.Core.Internal.Storage.InheritanceStoraging.EmptyInheritanceStorage",
            "SymOntoClay.Core.Internal.Htn.BuildPlanIterationPropertyStorage",
            "SymOntoClay.Core.Internal.Instances.PropertyInstance",
            "SymOntoClay.Core.Internal.Instances.AppInstance",
            "SymOntoClay.Core.Internal.DataResolvers.StatesResolver",
            "SymOntoClay.Core.Internal.DataResolvers.IdleActionsResolver",
            "SymOntoClay.Core.Internal.CodeModel.AppInstanceCodeItem",
            "SymOntoClay.Core.Internal.CodeModel.InheritanceItem",
            "SymOntoClay.Core.Internal.Storage.TriggersStoraging.TriggersStorage",
            "SymOntoClay.Core.Internal.CodeModel.Property",
            "SymOntoClay.Core.Internal.Storage.VarStoraging.VarStorage",
            "SymOntoClay.Core.Internal.Storage.PropertyStoraging.PropertyStorage",
            "SymOntoClay.Core.Internal.Storage.RelationStoraging.RelationsStorage",
            "SymOntoClay.Core.Internal.Storage.MethodsStoraging.MethodsStorage",
            "SymOntoClay.Core.Internal.Storage.ConstructorsStoraging.ConstructorsStorage",
            "SymOntoClay.Core.Internal.Storage.ActionsStoraging.ActionsStorage",
            "SymOntoClay.Core.Internal.Storage.StatesStoraging.StatesStorage",
            "SymOntoClay.Core.Internal.Storage.InheritanceStoraging.InheritanceStorage",
            "SymOntoClay.Core.Internal.Storage.SynonymsStoraging.SynonymsStorage",
            "SymOntoClay.Core.Internal.Storage.OperatorsStoraging.OperatorsStorage",
            "SymOntoClay.Core.Internal.Storage.ChannelsStoraging.ChannelsStorage",
            "SymOntoClay.Core.Internal.Storage.MetadataStoraging.MetadataStorage",
            "SymOntoClay.Core.Internal.Storage.FuzzyLogic.FuzzyLogicStorage",
            "SymOntoClay.Core.Internal.CommonNames.CommonNamesStorage",
            "SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging.IdleActionItemsStorage",
            "SymOntoClay.Core.Internal.Storage.TasksStoraging.TasksStorage",
            "SymOntoClay.Core.Internal.Storage.TasksStoraging.CommonTasksStorage`1",
            "SymOntoClay.Core.Internal.Storage.CompoundTaskInstanceStorage",
            "SymOntoClay.Core.Internal.DefaultSettingsOfCodeEntity",
            "SymOntoClay.Core.StorageUsingOptions",
            "SymOntoClay.Core.Internal.Storage.InheritancePublicFactsReplicator",
            "SymOntoClay.Core.Internal.Storage.SuperClassStorage",
            "SymOntoClay.Core.Internal.Storage.LocalStorage",
            "SymOntoClay.Core.Internal.CodeModel.Ast.Expressions.ConstValueAstExpression",
            "SymOntoClay.Core.Internal.CodeModel.CodeFile",
            "SymOntoClay.Core.Internal.CodeModel.App",
            "SymOntoClay.Core.Internal.CodeModel.NamedFunction",
            "SymOntoClay.Core.Internal.CodeModel.Ast.Statements.AstExpressionStatement",
            "SymOntoClay.Core.Internal.CodeModel.Ast.Expressions.BinaryOperatorAstExpression",
            "SymOntoClay.Core.Internal.CodeModel.StringValue",
            "SymOntoClay.Core.Internal.CodeModel.Ast.Statements.AstWaitStatement",
            "SymOntoClay.Core.Internal.IndexedData.ScriptingData.CompiledFunctionBody",
            "SymOntoClay.Core.Internal.IndexedData.ScriptingData.ScriptCommand",
            "SymOntoClay.Core.Internal.CodeModel.CompoundHtnTask",
            "SymOntoClay.Core.Internal.CodeModel.CompoundHtnTaskCase",
            "SymOntoClay.Core.Internal.CodeModel.CompoundHtnTaskCaseItem",
            "SymOntoClay.Core.Internal.CodeModel.PrimitiveHtnTask",
            "SymOntoClay.Core.Internal.CodeModel.PrimitiveHtnTaskOperator",
            "SymOntoClay.Core.Internal.CodeModel.Ast.Expressions.CallingFunctionAstExpression",
            "SymOntoClay.Core.Internal.Compiling.Internal.IntermediateScriptCommand",
            "SymOntoClay.Core.Internal.CodeModel.ExecutableCodeBlock",
            "SymOntoClay.Core.Internal.CodeModel.LogicalExecutableExpression",
            "SymOntoClay.Core.Internal.CodeModel.HostValue",
            "SymOntoClay.Core.Internal.CodeModel.InstanceValue",
            "SymOntoClay.Core.Internal.DataResolvers.TriggersResolver",
            "SymOntoClay.Core.Internal.DataResolvers.ConstructorsResolver",
            "SymOntoClay.Core.Internal.Instances.ExecutionCoordinator",
            "SymOntoClay.Core.Internal.Instances.BaseInstanceParentExecutionCoordinatorOnFinishedHandler",
            "SymOntoClay.Core.Internal.Instances.InternalRunners.PreConstructorsRunner",
            "SymOntoClay.Core.Internal.Instances.TaskInstances.CompoundTaskInstance",
            "SymOntoClay.Core.Internal.Instances.InternalRunners.ConstructorsRunner",
            "SymOntoClay.Core.Internal.CodeExecution.SyncThreadExecutor",
            "SymOntoClay.Core.Internal.Instances.InternalRunners.EnterLifecycleTriggersRunner",
            "SymOntoClay.Core.Internal.Instances.InternalRunners.FinalizationTriggersRunner",
            "SymOntoClay.Core.Internal.CodeExecution.CodeFrameAsyncExecutor",
            "SymOntoClay.Core.Internal.Compiling.Compiler",
            "SymOntoClay.Core.Internal.Instances.InstancesStorageComponent",
            "SymOntoClay.Core.Internal.Instances.InstancesStorageComponentOnFinishProcessWithoutDevicesHandler",
            "SymOntoClay.Core.Internal.Instances.InstancesStorageComponentOnFinishProcessWithDevicesHandler",
            "SymOntoClay.Core.Internal.DataResolvers.MetadataResolver",
            "SymOntoClay.ActiveObject.Functors.LoggedFunctorWithoutResult`2",
            "SymOntoClay.Core.Internal.Instances.ProcessInfo",
            "SymOntoClay.Core.Internal.CodeExecution.CodeFrame",
            "SymOntoClay.Core.Internal.CodeExecution.CodeFrameEvnPart",
            "SymOntoClay.Core.Internal.DataResolvers.StrongIdentifierLinearResolver",
            "SymOntoClay.ActiveObject.Threads.AsyncActiveOnceObject",
            "SymOntoClay.Core.Internal.Htn.BuildPlanIterationLocalCodeExecutionContext",
            "SymOntoClay.Core.Internal.DataResolvers.AnnotationsResolver",
            "SymOntoClay.Core.Internal.DataResolvers.DateTimeResolver",
            "SymOntoClay.Core.Internal.DataResolvers.StrongIdentifierExprValueResolver",
            "SymOntoClay.Core.Internal.DataResolvers.ValueResolvingHelper",
            "SymOntoClay.Core.Internal.Converters.ConverterFactToImperativeCode",
            "SymOntoClay.Core.Internal.DataResolvers.RelationsResolver",
            "SymOntoClay.Core.Internal.CodeExecution.AsyncThreadExecutor",
            "SymOntoClay.Core.Internal.CodeModel.PreConstructor",
            "SymOntoClay.Core.Internal.CodeModel.Operator",
            "SymOntoClay.Core.Internal.CodeExecution.BinaryOperatorSystemHandler",
            "SymOntoClay.Core.Internal.StandardLibrary.Operators.LeftRightStreamOperatorHandler",
            "SymOntoClay.Core.Internal.DataResolvers.ChannelsResolver"
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Monitor.Common.SerializationData.ThreadLoggerSerializationData"] = new List<string>()
            {
                "Parent",
                "ThreadId"
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
                "_commonNamesStorage",
                "_state",
                "_stateLockObj",
                "_logger"
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
                "_realStorageContext",
                "_lockObj",
                "_logicalStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_statesStorage",
                "_triggersStorage",
                "_inheritanceStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_varStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_onParentStorageChangedHandlersLockObj",
                "_onParentStorageChangedHandlers",
                "DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "_isDisposed",
                "_logger",
                "IsIsolated"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Functors.SerializationAnchor"] = new List<string>() 
            { 
                "_lockObj",
                "_functors"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.RealStorageContext"] = new List<string>() 
            { 
                "_onAddParentStorageHandlersLockObj",
                "_onAddParentStorageHandlers",
                "_onRemoveParentStorageHandlersLockObj",
                "_onRemoveParentStorageHandlers",
                "MainStorageContext",
                "LogicalStorage",
                "RelationsStorage",
                "MethodsStorage",
                "ConstructorsStorage",
                "ActionsStorage",
                "StatesStorage",
                "TriggersStorage",
                "InheritanceStorage",
                "SynonymsStorage",
                "OperatorsStorage",
                "ChannelsStorage",
                "MetadataStorage",
                "VarStorage",
                "FuzzyLogicStorage",
                "IdleActionItemsStorage",
                "TasksStorage",
                "PropertyStorage",
                "Logger",
                "Storage",
                "ParentCodeExecutionContext",
                "Parents",
                "InheritancePublicFactsReplicator",
                "KindOfGC",
                "EnableOnAddingFactEvent",
                "Disabled"
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
                "_dateTimeProvider",
                "_onChangedHandlersLockObj",
                "_onChangedHandlers",
                "_onChangedWithKeysHandlersLockObj",
                "_onChangedWithKeysHandlers",
                "_onAddingFactHandlerLockObj",
                "_onAddingFactHandlers",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
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
                "_kind",
                "_realStorageContext",
                "_lockObj",
                "_logicalStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_statesStorage",
                "_triggersStorage",
                "_inheritanceStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_varStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_onParentStorageChangedHandlersLockObj",
                "_onParentStorageChangedHandlers",
                "DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "_isDisposed",
                "_logger",
                "IsIsolated"
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
                "TriggersThreadPool",
                "Id",
                "SelfName",
                "AppFile",
                "ActiveObjectContext",
                "Storage",
                "Parser",
                "DataResolversFactory",
                "ConvertersFactory",
                "TypeConverter",
                "CommonNamesStorage",
                "InstancesStorage",
                "LoaderFromSourceCode",
                "ServicesFactory",
                "LogicQueryParseAndCache",
                "ModulesStorage"
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
                "_appInstanceStorageFactory",
                "_objectStorageFactory",
                "_stateStorageFactory",
                "_actionStorageFactory",
                "_rootTaskInstanceStorageFactory",
                "_strategicTaskInstanceStorageFactory",
                "_tacticalTaskInstanceStorageFactory",
                "_compoundTaskInstanceStorageFactory",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.AppInstanceStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.ObjectStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.StateStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.ActionStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.RootTaskInstanceStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.StrategicTaskInstanceStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.TacticalTaskInstanceStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.Factories.CompoundTaskInstanceStorageFactory"] = new List<string>() { };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.StandaloneStorage"] = new List<string>() 
            {
                "_context",
                "_additionalSourceCodePaths",
                "_storageComponent",
                "_storage",
                "_publicFactsStorage",
                "_worldPublicFactsStorage",
                "_deferredPublicFactsTexts",
                "_deferredPublicFactsInstances",
                "_deferredRemovedPublicFacts",
                "_deferredAddedCategories",
                "_deferredRemovedCategories",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.GlobalStorage"] = new List<string>() 
            {
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_kind",
                "_realStorageContext",
                "_lockObj",
                "_logicalStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_statesStorage",
                "_triggersStorage",
                "_inheritanceStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_varStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_onParentStorageChangedHandlersLockObj",
                "_onParentStorageChangedHandlers",
                "DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "_isDisposed",
                "_logger",
                "IsIsolated"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.LogicalStoraging.EmptyLogicalStorage"] = new List<string>()
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Htn.BuildPlanIterationStorage"] = new List<string>() 
            {
                "_parentStorage",
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
                "_state",
                "_stateLockObj",
                "_logger",
                "CodeItemsStoragesList"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.AppInstanceStorage"] = new List<string>() 
            {
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_kind",
                "_realStorageContext",
                "_lockObj",
                "_logicalStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_statesStorage",
                "_triggersStorage",
                "_inheritanceStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_varStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_onParentStorageChangedHandlersLockObj",
                "_onParentStorageChangedHandlers",
                "DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "_isDisposed",
                "_logger",
                "IsIsolated"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.NumberValue"] = new List<string>() 
            { 
                "_builtInSuperTypes",
                "_isFuzzy",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.InheritanceStoraging.EmptyInheritanceStorage"] = new List<string>() 
            {
                "_storage",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Htn.BuildPlanIterationPropertyStorage"] = new List<string>() 
            {
                "_storage",
                "_lockObj",
                "_allPropertiesList",
                "_propertiesDict",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.PropertyInstance"] = new List<string>() 
            {
                "_instance",
                "_context",
                "_logicalStorage",
                "_typeConverter",
                "IsReal",
                "Name",
                "Holder",
                "CodeItem",
                "_isArray",
                "_propertyGetMethodExecutable",
                "_value",
                "_factId",
                "_onChangedHandlersLockObj",
                "_onChangedHandlers",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.AppInstance"] = new List<string>() 
            {
                "_activeObjectContext",
                "_serializationAnchor",
                "_statesResolver",
                "_stateNameForAutomaticStart",
                "_activeStatesDict",
                "_mutuallyExclusiveStatesSet",
                "_statesLockObj",
                "_stateActivators",
                "_state",
                "_stateLockObj",
                "_logger",
                "_idleActionsResolver",
                "_codeItem",
                "Name",
                "_activeObjectContext",
                "_serializationAnchor",
                "_threadPool",
                "_context",
                "_globalTriggersStorage",
                "_parentStorage",
                "_storage",
                "_localCodeExecutionContext",
                "_parentExecutionCoordinator",
                "_triggersResolver",
                "_constructorsResolver",
                "_inheritanceResolver",
                "_instanceState",
                "_logicConditionalTriggersList",
                "_addingFactNonConditionalTriggerInstancesList",
                "_addingFactConditionalTriggerInstancesList",
                "_executionCoordinator",
                "_baseInstanceParentExecutionCoordinatorOnFinishedHandler",
                "_childInstances",
                "_parentInstance",
                "_childInstancesLockObj",
                "_superClassesStorages",
                "_superClassesStoragesLockObj",
                "_preConstructorsRunner",
                "_constructors",
                "_enterLifecycleTriggersRunner",
                "_finalizationTriggersRunner",
                "RootTasks"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.StatesResolver"] = new List<string>()
            {
                "_synonymsResolver",
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.IdleActionsResolver"] = new List<string>() 
            {
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.AppInstanceCodeItem"] = new List<string>() 
            { 
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
                "SystemValue",
                "InternalSystemId",
                "WhereSection",
                "Annotations",
                "RootTasks",
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
                "ImportsList"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.InheritanceItem"] = new List<string>() 
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
                "Id",
                "SubName",
                "SuperName",
                "Rank",
                "IsSystemDefined",
                "KeysOfPrimaryRecords",
                "InternalSystemId",
                "WhereSection",
                "Annotations"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.TriggersStoraging.TriggersStorage"] = new List<string>()
            { 
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_lockObj",
                "_parentTriggersStoragesList",
                "_systemEventsInfoDict",
                "_logicConditionalsDict",
                "_addFactsDict",
                "_namedTriggerInstancesList",
                "_namedTriggerInstancesDict",
                "_onNamedTriggerInstanceChangedHandlersLockObj",
                "_onNamedTriggerInstanceChangedHandlers",
                "_onNamedTriggerInstanceChangedWithKeysHandlersLockObj",
                "_onNamedTriggerInstanceChangedWithKeysHandlers",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.Property"] = new List<string>() 
            { 
                "KindOfProperty",
                "TypesList",
                "DefaultValue",
                "GetStatements",
                "GetCompiledFunctionBody",
                "IsAnonymous",
                "InheritanceItems",
                "CodeFile",
                "ParentCodeEntity",
                "SubItems",
                "_onNameChangedHandlersLockObj",
                "_onNameChangedHandlers",
                "_name",
                "_holder",
                "_typeOfAccess",
                "Directives",
                "ActivatingConditions",
                "DeactivatingConditions",
                "IdleActionItems",
                "Priority",
                "ImportsList",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.VarStoraging.VarStorage"] = new List<string>() 
            {
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_lockObj",
                "_onChangedHandlersLockObj",
                "_onChangedHandlers",
                "_onChangedWithKeysHandlersLockObj",
                "_onChangedWithKeysHandlers",
                "_parentVarStoragesList",
                "_variablesDict",
                "_localVariablesDict",
                "_allVariablesList",
                "_systemVariables",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.PropertyStoraging.PropertyStorage"] = new List<string>()
            { 
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_lockObj",
                "_onChangedHandlersLockObj",
                "_onChangedHandlers",
                "_onChangedWithKeysHandlersLockObj",
                "_onChangedWithKeysHandlers",
                "_parentPropertyStoragesList",
                "_allPropertiesList",
                "_propertiesDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.RelationStoraging.RelationsStorage"] = new List<string>() 
            { 
                "_lockObj",
                "_itemsDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.MethodsStoraging.MethodsStorage"] = new List<string>()
            {
                "_lockObj",
                "_namedFunctionsDict",
                "_localNamedFunctionsDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.ConstructorsStoraging.ConstructorsStorage"] = new List<string>() 
            {
                "_constructorsLockObj",
                "_constructorsDict",
                "_preConstructorsLockObj",
                "_preConstructorsDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.ActionsStoraging.ActionsStorage"] = new List<string>() 
            {
                "_lockObj",
                "_actionsDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.StatesStoraging.StatesStorage"] = new List<string>() 
            {
                "_lockObj",
                "_statesDict",
                "_activationInfoDict",
                "_statesList",
                "_activationInfoList",
                "_stateNamesList",
                "_defaultStateName",
                "_mutuallyExclusiveStatesSetsList",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.InheritanceStoraging.InheritanceStorage"] = new List<string>() 
            {
                "_lockObj",
                "_factsIdRegistryLockObj",
                "_inheritancePublicFactsReplicator",
                "_nonIndexedInfo",
                "_factsIdRegistry",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.SynonymsStoraging.SynonymsStorage"] = new List<string>() 
            {
                "_lockObj",
                "_synonymsDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.OperatorsStoraging.OperatorsStorage"] = new List<string>() 
            {
                "_lockObj",
                "_nonIndexedInfo",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.ChannelsStoraging.ChannelsStorage"] = new List<string>() 
            {
                "_lockObj",
                "_nonIndexedInfo",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.MetadataStoraging.MetadataStorage"] = new List<string>() 
            {
                "_lockObj",
                "_mainCodeEntity",
                "_codeEntitiesDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.FuzzyLogic.FuzzyLogicStorage"] = new List<string>()
            {
                "_lockObj",
                "_commonNamesStorage",
                "_valuesDict",
                "_defaultOperatorsDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CommonNames.CommonNamesStorage"] = new List<string>() 
            {
                "_context",
                "WorldName",
                "AppName",
                "ClassName",
                "ActionName",
                "StateName",
                "DefaultHolder",
                "SelfSystemVarName",
                "HostSystemVarName",
                "AnonymousLogicalVarName",
                "TargetLogicalVarName",
                "SelfName",
                "DefaultCtorName",
                "RandomConstraintName",
                "NearestConstraintName",
                "TimeoutAttributeName",
                "PriorityAttributeName",
                "AnyTypeName",
                "BooleanTypeName",
                "FuzzyTypeName",
                "NumberTypeName",
                "TrueValueLiteral",
                "FalseValueLiteral",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.IdleActionItemsStoraging.IdleActionItemsStorage"] = new List<string>() 
            {
                "_lockObj",
                "_itemsDict",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.TasksStoraging.TasksStorage"] = new List<string>() 
            {
                "_rootTasksStorage",
                "_strategicTasksStorage",
                "_tacticalTasksStorage",
                "_compoundCommonTasksStorage",
                "_primitiveCommonTasksStorage",
                "_kind",
                "_realStorageContext",
                "_mainStorageContext",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.TasksStoraging.CommonTasksStorage`1"] = new List<string>() 
            {
                "_tasksList",
                "_tasksDict",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.CompoundTaskInstanceStorage"] = new List<string>() 
            {
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_kind",
                "_realStorageContext",
                "_lockObj",
                "_logicalStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_statesStorage",
                "_triggersStorage",
                "_inheritanceStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_varStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_onParentStorageChangedHandlersLockObj",
                "_onParentStorageChangedHandlers",
                "DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "_isDisposed",
                "_logger",
                "IsIsolated"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DefaultSettingsOfCodeEntity"] = new List<string>() 
            { 
                "WhereSection",
                "Holder",
                "TypeOfAccess"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.StorageUsingOptions"] = new List<string>() 
            {
                "Storage",
                "UseFacts",
                "UseInheritanceFacts",
                "UseProductions",
                "MaxDeph",
                "Priority"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.InheritancePublicFactsReplicator"] = new List<string>()
            {
                "_lockObj",
                "_context",
                "_publicFactsStorage",
                "_publicInheritanceStorage",
                "_inheritanceResolver",
                "_resolverOptions",
                "_standardCoreFactsBuilder",
                "_foundInheritanceKeysList",
                "_localCodeExecutionContext",
                "_logicQueryParseAndCache",
                "_selfName",
                "_selfNameForFacts",
                "_factsIdDict",
                "_inheritanceItemsDict",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.SuperClassStorage"] = new List<string>() 
            {
                "_targetClassName",
                "_instanceName",
                "_instance",
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_kind",
                "_realStorageContext",
                "_lockObj",
                "_logicalStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_statesStorage",
                "_triggersStorage",
                "_inheritanceStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_varStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_onParentStorageChangedHandlersLockObj",
                "_onParentStorageChangedHandlers",
                "DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "_isDisposed",
                "_logger",
                "IsIsolated"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Storage.LocalStorage"] = new List<string>()
            {
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_kind",
                "_realStorageContext",
                "_lockObj",
                "_logicalStorage",
                "_relationsStorage",
                "_methodsStorage",
                "_constructorsStorage",
                "_actionsStorage",
                "_statesStorage",
                "_triggersStorage",
                "_inheritanceStorage",
                "_synonymsStorage",
                "_operatorsStorage",
                "_channelsStorage",
                "_metadataStorage",
                "_varStorage",
                "_fuzzyLogicStorage",
                "_idleActionItemsStorage",
                "_tasksStorage",
                "_propertyStorage",
                "_onParentStorageChangedHandlersLockObj",
                "_onParentStorageChangedHandlers",
                "DefaultSettingsOfCodeEntity",
                "CodeItemsStoragesList",
                "_isDisposed",
                "_logger",
                "IsIsolated"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.Ast.Expressions.ConstValueAstExpression"] = new List<string>() 
            { 
                "Value",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.CodeFile"] = new List<string>() 
            {
                "FileName",
                "IsMain",
                "IsLocator",
                "CodeEntities"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.App"] = new List<string>() 
            {
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
                "RootTasks",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.NamedFunction"] = new List<string>() 
            { 
                "Arguments",
                "TypesList",
                "Statements",
                "CompiledFunctionBody",
                "_argumentsDict",
                "_iArgumentsList",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.Ast.Statements.AstExpressionStatement"] = new List<string>() 
            { 
                "Expression",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.Ast.Expressions.BinaryOperatorAstExpression"] = new List<string>() 
            { 
                "KindOfOperator",
                "Left",
                "Right",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.StringValue"] = new List<string>() 
            { 
                "SystemValue",
                "_toRuleInstanceValueLockObj",
                "_ruleInstance",
                "_usedSystemValueForRuleInstanceValue",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.Ast.Statements.AstWaitStatement"] = new List<string>()
            { 
                "Items",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.IndexedData.ScriptingData.CompiledFunctionBody"] = new List<string>() 
            { 
                "Commands",
                "SEH"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.IndexedData.ScriptingData.ScriptCommand"] = new List<string>() 
            {
                "OperationCode",
                "Position",
                "Value",
                "AnnotatedItem",
                "CompoundTask",
                "TargetPosition",
                "KindOfOperator",
                "CountParams"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.CompoundHtnTask"] = new List<string>() 
            {
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
                "Before",
                "After",
                "Backgrounds",
                "Cases",
                "Precondition",
                "PreconditionExpression",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.CompoundHtnTaskCase"] = new List<string>() 
            {
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
                "Condition",
                "ConditionExpression",
                "Items",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.CompoundHtnTaskCaseItem"] = new List<string>() 
            { 
                "Name" 
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.PrimitiveHtnTask"] = new List<string>() 
            { 
                "Operator",
                "Effects",
                "ExpectedEffects",
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
                "Precondition",
                "PreconditionExpression",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.PrimitiveHtnTaskOperator"] = new List<string>() 
            { 
                "Statement",
                "IntermediateCommandsList"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.Ast.Expressions.CallingFunctionAstExpression"] = new List<string>() 
            {
                "Left",
                "Parameters",
                "IsAsync",
                "IsChild",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Compiling.Internal.IntermediateScriptCommand"] = new List<string>() 
            {
                "OperationCode",
                "Position",
                "Value",
                "AnnotatedItem",
                "CompoundTask",
                "JumpToMe",
                "KindOfOperator",
                "CountParams",
                "SEHGroup"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.ExecutableCodeBlock"] = new List<string>() 
            {
                "Statements",
                "CompiledFunctionBody",
                "_iArgumentsList",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.LogicalExecutableExpression"] = new List<string>() 
            { 
                "_iArgumentsList",
                "TypesList",
                "Expression",
                "CompiledFunctionBody",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.HostValue"] = new List<string>() 
            { 
                "_getMemberLockObj",
                "_membersDict",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.InstanceValue"] = new List<string>() 
            { 
                "InstanceInfo",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.TriggersResolver"] = new List<string>() 
            { 
                "_synonymsResolver",
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.ConstructorsResolver"] = new List<string>() 
            {
                "_emptyConstructorsList",
                "_emptyPreConstructorsList",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.ExecutionCoordinator"] = new List<string>() 
            {
                "_instance",
                "_lockObj",
                "Id",
                "_isFinished",
                "_executionStatus",
                "_ruleInstance",
                "_onFinishedHandlersLockObj",
                "_onFinishedHandlers",
                "_processInfosLockObj",
                "_processInfosList"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.BaseInstanceParentExecutionCoordinatorOnFinishedHandler"] = new List<string>() 
            {
                "_logger",
                "_executionCoordinator",
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.InternalRunners.PreConstructorsRunner"] = new List<string>() 
            {
                "_logger",
                "_context",
                "_triggersResolver",
                "_instance",
                "_instanceWithChangingState",
                "_instanceName",
                "_holder",
                "_localCodeExecutionContext",
                "_executionCoordinator",
                "_constructorsResolver",
                "_threadExecutor",
                "_state"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.TaskInstances.CompoundTaskInstance"] = new List<string>() 
            {
                "_baseCompoundHtnTask",
                "_compoundHtnTaskBackgroundTriggerInstancesList",
                "_codeItem",
                "Name",
                "_activeObjectContext",
                "_serializationAnchor",
                "_threadPool",
                "_context",
                "_globalTriggersStorage",
                "_parentStorage",
                "_storage",
                "_localCodeExecutionContext",
                "_parentExecutionCoordinator",
                "_triggersResolver",
                "_constructorsResolver",
                "_inheritanceResolver",
                "_instanceState",
                "_logicConditionalTriggersList",
                "_addingFactNonConditionalTriggerInstancesList",
                "_addingFactConditionalTriggerInstancesList",
                "_executionCoordinator",
                "_baseInstanceParentExecutionCoordinatorOnFinishedHandler",
                "_childInstances",
                "_parentInstance",
                "_childInstancesLockObj",
                "_superClassesStorages",
                "_superClassesStoragesLockObj",
                "_preConstructorsRunner",
                "_constructors",
                "_enterLifecycleTriggersRunner",
                "_finalizationTriggersRunner",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.InternalRunners.ConstructorsRunner"] = new List<string>() 
            {
                "_logger",
                "_context",
                "_triggersResolver",
                "_instance",
                "_instanceWithChangingState",
                "_instanceName",
                "_holder",
                "_localCodeExecutionContext",
                "_executionCoordinator",
                "_storage",
                "_constructorsResolver",
                "_threadExecutor",
                "_state"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.SyncThreadExecutor"] = new List<string>() 
            {
                "_context",
                "_codeFrameService",
                "_codeFrameAsyncExecutor",
                "_threadId",
                "_projectLoader",
                "_globalStorage",
                "_globalLogicalStorage",
                "_hostListener",
                "_instancesStorage",
                "_operatorsResolver",
                "_logicalValueLinearResolver",
                "_numberValueLinearResolver",
                "_strongIdentifierLinearResolver",
                "_varsResolver",
                "_propertiesResolver",
                "_methodsResolver",
                "_constructorsResolver",
                "_logicalSearchResolver",
                "_statesResolver",
                "_annotationsResolver",
                "_inheritanceResolver",
                "_dateTimeResolver",
                "_strongIdentifierExprValueResolver",
                "_valueResolvingHelper",
                "_typeConverter",
                "_converterFactToImperativeCode",
                "_dateTimeProvider",
                "_codeFrames",
                "_currentCodeFrame",
                "_executionCoordinator",
                "_currentInstance",
                "_currentVarStorage",
                "_currentPropertyStorage",
                "_currentError",
                "_isCanceled",
                "_endOfTargetDuration",
                "_waitedThreadExecutorsList",
                "_waitedProcessInfoList",
                "_defaultCtorName",
                "_timeoutName",
                "_priorityName",
                "ExternalReturn",
                "_activeObject",
                "_onCompletedHandlersLockObj",
                "_onCompletedHandlers",
                "_processAddLifeCycleEventParametersCount",
                "_defaultTimeoutCancellationMode",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.AsyncThreadExecutor"] = new List<string>()
            {
                "_context",
                "_codeFrameService",
                "_codeFrameAsyncExecutor",
                "_threadId",
                "_projectLoader",
                "_globalStorage",
                "_globalLogicalStorage",
                "_hostListener",
                "_instancesStorage",
                "_operatorsResolver",
                "_logicalValueLinearResolver",
                "_numberValueLinearResolver",
                "_strongIdentifierLinearResolver",
                "_varsResolver",
                "_propertiesResolver",
                "_methodsResolver",
                "_constructorsResolver",
                "_logicalSearchResolver",
                "_statesResolver",
                "_annotationsResolver",
                "_inheritanceResolver",
                "_dateTimeResolver",
                "_strongIdentifierExprValueResolver",
                "_valueResolvingHelper",
                "_typeConverter",
                "_converterFactToImperativeCode",
                "_dateTimeProvider",
                "_codeFrames",
                "_currentCodeFrame",
                "_executionCoordinator",
                "_currentInstance",
                "_currentVarStorage",
                "_currentPropertyStorage",
                "_currentError",
                "_isCanceled",
                "_endOfTargetDuration",
                "_waitedThreadExecutorsList",
                "_waitedProcessInfoList",
                "_defaultCtorName",
                "_timeoutName",
                "_priorityName",
                "ExternalReturn",
                "_activeObject",
                "_onCompletedHandlersLockObj",
                "_onCompletedHandlers",
                "_processAddLifeCycleEventParametersCount",
                "_defaultTimeoutCancellationMode",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.InternalRunners.EnterLifecycleTriggersRunner"] = new List<string>() 
            {
                "_logger",
                "_context",
                "_triggersResolver",
                "_instance",
                "_instanceName",
                "_holder",
                "_localCodeExecutionContext",
                "_executionCoordinator",
                "_storage",
                "_kindOfSystemEvent",
                "_normalOrder",
                "_runOnce",
                "_wasRun",
                "_threadExecutor",
                "_lockObj"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.InternalRunners.FinalizationTriggersRunner"] = new List<string>() 
            {
                "_wasRun",
                "_lockObj",
                "_logger",
                "_context",
                "_triggersResolver",
                "_instance",
                "_holder",
                "_localCodeExecutionContext",
                "_storage",
                "_finalizationExecutionCoordinator",
                "_leaveLifecycleTriggersRunner"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.CodeFrameAsyncExecutor"] = new List<string>() 
            { 
                "_context",
                "_codeFrameService",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Compiling.Compiler"] = new List<string>() 
            {
                "_context",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.InstancesStorageComponent"] = new List<string>() 
            {
                "_instancesStorageComponentOnFinishProcessWithoutDevicesHandler",
                "_instancesStorageComponentOnFinishProcessWithDevicesHandler",
                "_context",
                "_metadataResolver",
                "_activeObjectContext",
                "_threadPool",
                "_serializationAnchor",
                "_registryLockObj",
                "_processLockObj",
                "_projectLoader",
                "_namesDict",
                "_rootInstanceInfo",
                "_processesInfoList",
                "_processesInfoByDevicesDict",
                "_commonNamesStorage",
                "_onIdleHandlersLockObj",
                "_onIdleHandlers",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.InstancesStorageComponentOnFinishProcessWithoutDevicesHandler"] = new List<string>() 
            { 
                "_instancesStorageComponent"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.InstancesStorageComponentOnFinishProcessWithDevicesHandler"] = new List<string>()
            {
                "_instancesStorageComponent"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.MetadataResolver"] = new List<string>() 
            { 
                "_synonymsResolver",
                "_defaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Functors.LoggedFunctorWithoutResult`2"] = new List<string>() 
            { 
                "_functorId",
                "_action",
                "_arg1",
                "_arg2",
                "_arg3",
                "_asyncActiveOnceObject",
                "_serializationAnchor"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Instances.ProcessInfo"] = new List<string>() 
            { 
                "CodeFrame",
                "_devices",
                "_friends",
                "_cancellationContext",
                "_threadPool",
                "_activeObjectContext",
                "_serializationAnchor",
                "Id",
                "EndPointName",
                "_onFinishHandlersLockObj",
                "_onFinishHandlers",
                "_onCompleteHandlersLockObj",
                "_onCompleteHandlers",
                "_onWeakCanceledHandlersLockObj",
                "_onWeakCanceledHandlers",
                "Priority",
                "_statusLockObj",
                "_parentAndChildrenLockObj",
                "_status",
                "_parentProcessInfo",
                "_childrenProcessInfoList",
                "_removedChildrenProcessInfoList",
                "_onFinishHandlersList",
                "_onCompleteHandlersList",
                "_onWeakCanceledHandlersList",
                "_isDisposed"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.CodeFrame"] = new List<string>() 
            {
                "CompiledFunctionBody",
                "CurrentPosition",
                "State",
                "CurrentSEHGroup",
                "ValuesStack",
                "SEHStack",
                "LocalContext",
                "ProcessInfo",
                "Metadata",
                "Arguments",
                "CallMethodId",
                "Instance",
                "ExecutionCoordinator",
                "SpecialMark",
                "TargetDuration",
                "EndOfTargetDuration",
                "TimeoutCancellationMode",
                "CalledCtorsList",
                "PutToValueStackAfterReturningBack",
                "NeedsExecCallEvent",
                "LastProcessStatus",
                "CompleteAnnotationSystemEvent",
                "CancelAnnotationSystemEvent",
                "WeakCancelAnnotationSystemEvent",
                "ErrorAnnotationSystemEvent",
                "PseudoSyncTask",
                "CodeFrameEvnPartsStack",
                "CompoundTaskInstance",
                "_callMode",
                "ForParameterValueResolving",
                "TakingValuesState",
                "ResolvingParameterValues",
                "CurrentPositionOfResolvingParameter",
                "ResolvedPositionedParameterValues",
                "ResolvedNamedParameterValues",
                "CurrentResolvedParameterValue",
                "CurrentCaller",
                "CurrentKindOfOperator",
                "KindOfParameters",
                "ParametersCount",
                "CurrentFunctionCallMethodId",
                "BaseCompoundTaskInstance"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.CodeFrameEvnPart"] = new List<string>() 
            {
                "LocalContext",
                "Metadata",
                "Instance",
                "ExecutionCoordinator",
                "CompoundTaskInstance"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.StrongIdentifierLinearResolver"] = new List<string>() 
            {
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.ActiveObject.Threads.AsyncActiveOnceObject"] = new List<string>() 
            {
                "_context",
                "_threadPool",
                "_cancellationContext",
                "_logger",
                "_lockObj",
                "ObjectWithOnceRunMethod",
                "_isWaited",
                "_isExited",
                "_task",
                "_onCompletedHandlersCollection",
                "_isDisposed"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Htn.BuildPlanIterationLocalCodeExecutionContext"] = new List<string>() 
            { 
                "_context",
                "Parent",
                "_storage",
                "KindOfAddFactResult"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.AnnotationsResolver"] = new List<string>() 
            {
                "_synonymsResolver",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.DateTimeResolver"] = new List<string>() 
            {
                "_dateTimeProvider",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.StrongIdentifierExprValueResolver"] = new List<string>() 
            {
                "_propertiesResolver",
                "_fuzzyLogicResolver",
                "_varsResolver",
                "_trueValueLiteral",
                "_falseValueLiteral",
                "DefaultOptions",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.ValueResolvingHelper"] = new List<string>() 
            {
                "_context",
                "_varsResolver",
                "_strongIdentifierExprValueResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.Converters.ConverterFactToImperativeCode"] = new List<string>() 
            {
                "_context",
                "_compiler",
                "_relationsResolver",
                "_actName",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.RelationsResolver"] = new List<string>() 
            {
                "DefaultOptions",
                "_synonymsResolver",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.PreConstructor"] = new List<string>() 
            {
                "Arguments",
                "TypesList",
                "Statements",
                "CompiledFunctionBody",
                "_argumentsDict",
                "_iArgumentsList",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeModel.Operator"] = new List<string>() 
            {
                "KindOfOperator",
                "IsSystemDefined",
                "Statements",
                "CompiledFunctionBody",
                "SystemHandler",
                "Arguments",
                "_argumentsDict",
                "_iArgumentsList",
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
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.CodeExecution.BinaryOperatorSystemHandler"] = new List<string>() 
            { 
                "_leftOperandKey",
                "_rightOperandKey",
                "_operatorHandler"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.StandardLibrary.Operators.LeftRightStreamOperatorHandler"] = new List<string>() 
            { 
                "_engineContext",
                "_channelsResolver",
                "_valueResolvingHelper",
                "_logger"
            };
            _tmpProcessedMembersOfTypes["SymOntoClay.Core.Internal.DataResolvers.ChannelsResolver"] = new List<string>() 
            { 
                "_synonymsResolver",
                "_context",
                "_inheritanceResolver",
                "_state",
                "_stateLockObj",
                "_logger"
            };
        }
#endif
    }
}
