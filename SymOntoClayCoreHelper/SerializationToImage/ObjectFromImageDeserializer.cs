using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectFromImageDeserializer: IObjectDeserializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjectFromImageDeserializer(IObjectDeserialializationFactory objectDeserialializationFactory, IDataCardDictionary objectsDataCardDictionary, IDataCardDictionary rootObjectsAndSettingsDataCardDictionary, IStructuralContext structuralContext, ISerializedTypesPool serializedTypesPool, ITypesHelper typesHelper)
        {
            _objectDeserialializationFactory = objectDeserialializationFactory;
            _objectsDataCardDictionary = objectsDataCardDictionary;
            _rootObjectsAndSettingsDataCardDictionary = rootObjectsAndSettingsDataCardDictionary;
            _structuralContext = structuralContext;
            _serializedTypesPool = serializedTypesPool;
            _typesHelper = typesHelper;

#if DEBUG
            InitTmpProcessedMembersOfTypes();
#endif
        }

        private readonly IObjectDeserialializationFactory _objectDeserialializationFactory;
        private readonly IDataCardDictionary _objectsDataCardDictionary;
        private readonly IDataCardDictionary _rootObjectsAndSettingsDataCardDictionary;
        private readonly IStructuralContext _structuralContext;
        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly ITypesHelper _typesHelper;

        /// <inheritdoc/>
        public object DeserializeValue(SerializedValue serializedValue, ObjMemberRef objMember = null)
        {
#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var obj = _objectDeserialializationFactory.GetValue(serializedValue);

#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            if (obj == null)
            {
                return null;
            }

            var type = obj.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
            _logger.Info($"type.IsArray = {type.IsArray}");
#endif

            var dataCard = GetDataCardByHeader(serializedValue, type);

#if DEBUG
            _logger.Info($"dataCard = {dataCard}");
#endif

            if (type.IsEnum)
            {
                return DeserializePrimitiveType(type, serializedValue);
            }

            if (type.IsArray)
            {
                return DeserializeArray(obj, type, dataCard as ArrayCard);
            }

            if (type.FullName.StartsWith("System.Action"))
            {
                return DeserializeAction(obj, type, objMember, dataCard as ActionCard);
            }

            if (type.FullName.StartsWith("System.Func"))
            {
                return DeserializeAction(obj, type, objMember, dataCard as ActionCard);
            }

            switch (type.FullName)
            {
                case "System.Object":
                    return DeserializeBareObject(obj, type, dataCard as ClassCard);

                case "System.Threading.CancellationTokenSource":
                    throw new NotImplementedException("C44E3BFE-ACF8-46EB-9059-0665A0BAB1E7");

                case "System.Threading.CancellationTokenSource+Linked1CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+Linked2CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+LinkedNCancellationTokenSource":
                    throw new NotImplementedException("C2A948D2-F5FE-413C-BE3E-BABAA04EA0E1");

                case "System.Threading.CancellationToken":
                    throw new NotImplementedException("C3F3E1DF-33BD-4C55-8B8D-62E4E79D21B0");

                case "System.Threading.ManualResetEvent":
                    return DeserializeManualResetEvent(obj, type, dataCard as ExternalManualResetEventClassCard);

                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Decimal":
                case "System.Double":
                case "System.Boolean":
                case "System.String":
                case "System.Char":
                case "System.DateTime":
                case "System.DateOnly":
                case "System.TimeOnly":
                case "System.TimeSpan":
                case "System.Guid":
                    return DeserializePrimitiveType(type, serializedValue);

                case "System.Type":
                case "System.RuntimeType":
                    return DeserializeReflectionType(obj, type, dataCard as ReflectionTypeCard);
            }

            var fullShortTypeName = $"{type.Namespace}.{type.Name}";

#if DEBUG
            //_logger.Info($"fullShortTypeName = {fullShortTypeName}");
#endif

            switch (fullShortTypeName)
            {
                case "System.Collections.Generic.List`1":
                    return DeserializeGenericList(obj, type, dataCard as ListCard);

                case "System.Collections.Generic.Stack`1":
                    return DeserializeGenericStack(obj, type, dataCard as StackCard);

                case "System.Collections.Generic.Queue`1":
                    return DeserializeGenericQueue(obj, type, dataCard as QueueCard);

                case "System.Collections.Generic.HashSet`1":
                    return DeserializeHashSet(obj, type, dataCard as HashSetCard);

                case "System.Collections.Generic.Dictionary`2":
                    return DeserializeGenericDictionary(obj, type, dataCard as DictionaryCard);

                default:
                    if (type.FullName.StartsWith("System.Threading.") ||
                        type.FullName.StartsWith("System.Collections."))
                    {
                        throw new NotImplementedException("C8E00923-0AF2-448A-8B42-8B037C442532");
                    }

                    return DeserializeComposite(obj, type, dataCard);
            }
        }

        private IDataCardWithHeader GetDataCardByHeader(SerializedValue header, Type type)
        {
#if DEBUG
            _logger.Info($"header = {header}");
            _logger.Info($"type?.FullName = {type?.FullName}");
#endif

            if (header.KindOfSerializedValue == KindOfSerializedValue.Null)
            {
                return null;
            }

            if (header.KindOfSerializedValue == KindOfSerializedValue.ExternalValue)
            {
                return null;
            }

            if (_typesHelper.GetKindOfSerializedValue(type) == KindOfSerializedValue.Literal)
            {
                return null;
            }

            if(_objectsDataCardDictionary.TryGetDataCardByHeader(header, out var card))
            {
                return card;
            }

            if(_rootObjectsAndSettingsDataCardDictionary.TryGetDataCardByHeader(header, out card))
            {
                return card;
            }

            throw new NotImplementedException("C474E318-39B4-4DAA-8CE2-EF131FA95FE9");
        }

        private object DeserializePrimitiveType(Type type, SerializedValue header)
        {
            return _typesHelper.FromString(type, header.Literal);
        }

        private object DeserializeReflectionType(object obj, Type type, ReflectionTypeCard card)
        {
            throw new NotImplementedException("C800E9B2-9E20-402B-A3B2-EB025A76E76E");
        }

        private object DeserializeBareObject(object obj, Type type, ClassCard card)
        {
            throw new NotImplementedException("C11674D7-5BD0-4AA7-811F-D0B8495CD505");
        }

        private object DeserializeArray(object obj, Type type, ArrayCard card)
        {
            throw new NotImplementedException("C3A41E1D-BC85-453B-A6B0-8D84105B5E4A");
        }

        private object DeserializeGenericList(object obj, Type type, ListCard card)
        {
            throw new NotImplementedException("C9CA6832-F3F1-4724-9043-28156FB104C7");
        }

        private object DeserializeGenericStack(object obj, Type type, StackCard card)
        {
            throw new NotImplementedException("C69DBB7C-2834-41D5-9568-D196D37DC196");
        }

        private object DeserializeGenericQueue(object obj, Type type, QueueCard card)
        {
            throw new NotImplementedException("C138422C-F405-4A36-8AB7-C4A5A9455BFB");
        }

        private object DeserializeHashSet(object obj, Type type, HashSetCard card)
        {
            throw new NotImplementedException("C2CD8D58-AC3D-451E-8C6B-11D02F521864");
        }

        private object DeserializeGenericDictionary(object obj, Type type, DictionaryCard card)
        {
            throw new NotImplementedException("C7F18F76-A497-4A99-A950-8EEE7F2EBA16");
        }

        private object DeserializeComposite(object obj, Type type, IDataCardWithHeader card)
        {
#if DEBUG
            TmpCheckProcessedTypes("DAFDB7EA-1D12-4619-B3F5-1028016968BC", type);
#endif

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            _logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
#endif

            switch (kindOfStructuralObject)
            {
                case KindOfStructuralObject.WorldRoot:
                case KindOfStructuralObject.WorldComponent:
                    return DeserializeKeyWorldComponent(obj, type, card as KeyWorldComponentClassCard);

                case KindOfStructuralObject.WorldSettings:
                    return DeserializeWorldSettings(obj, type, card as ExternalClassCard);

                case KindOfStructuralObject.SerializeWithSerializationDataCreation:
                    return DeserializeWithSerializationDataCreation(obj, type, card as ClassCardWithSerializationData);

                case KindOfStructuralObject.UsualObject:
                    return DeserializeUsualObject(obj, type, card as ClassCard);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStructuralObject), kindOfStructuralObject, "2F0E3A83-964D-4E30-B083-FFAC86976C3D");
            }
        }

        private object DeserializeKeyWorldComponent(object obj, Type type, KeyWorldComponentClassCard card)
        {
#if DEBUG
            _logger.Info($"card = {card}");
#endif

            foreach(var item in card.FieldsWithSerializedMembers)
            {
#if DEBUG
                _logger.Info($"name (1) = {item.Item1}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("72672943-9B36-46C9-91AF-53F701D4D105", type, item.Item1);
#endif

                ProcessField(obj, item);

                throw new NotImplementedException("C600854B-7E09-4B99-A29F-87A20272AA1D");
            }

            foreach (var item in card.FieldsWithChildren)
            {
#if DEBUG
                _logger.Info($"item.Item1 (2) = {item.Item1}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("7B69766C-9D83-4BFC-9246-CEA171935042", type, item.Item1);
#endif

                ProcessField(obj, item);

                throw new NotImplementedException("C179CFA3-14DC-4872-86F5-A35902FA72EF");
            }

            foreach (var item in card.OtherFields)
            {
#if DEBUG
                _logger.Info($"item.Item1 (3) = {item.Item1}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("05A68523-5259-4EDC-A439-53C3D2B61277", type, item.Item1);
#endif

                ProcessField(obj, item);

                throw new NotImplementedException("C13FCEC2-ED94-4A21-8B8D-1DF3148F39FC");
            }

            foreach (var item in card.Properties)
            {
                var name = item.Item1;

#if DEBUG
                _logger.Info($"name (4) = {name}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("02018B0B-AD73-4513-BCFB-443049C3A115", type, name);
#endif

                throw new NotImplementedException("C51E0F23-E34D-4B7C-9CD3-F8307BC33574");
            }

            throw new NotImplementedException("C8CA053E-8B82-40B4-90DC-B100748AE0A5");
        }

        private object DeserializeWorldSettings(object obj, Type type, ExternalClassCard card)
        {
#if DEBUG
            _logger.Info($"card = {card}");
#endif

            if(card.Fields != null)
            {
                foreach (var item in card.Fields)
                {
#if DEBUG
                    _logger.Info($"item.Item1 (1) = {item.Item1}");
#endif

#if DEBUG
                    TmpCheckProcessedMembersOfTypes("C440A66A-CC4E-46F0-A1F6-36CF018F5733", type, item.Item1);
#endif

                    ProcessField(obj, item);

                    //throw new NotImplementedException("CC55C6C9-BB5E-40E6-97EB-2E0776A7A5AA");
                }
            }

            foreach (var item in card.Properties)
            {
                var name = item.Item1;

#if DEBUG
                _logger.Info($"name (2) = {name}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("C14E96E8-2F4B-4ECA-AE67-89ABBDD69475", type, name);
#endif

                ProcessProperty(obj, item);

                //throw new NotImplementedException("CC190AD1-8A2D-458B-AF36-5D31C8E3F3EF");
            }

            throw new NotImplementedException("C31353B5-CE4C-477A-AC83-42B730137FF7");
        }

        private object DeserializeWithSerializationDataCreation(object obj, Type type, ClassCardWithSerializationData card)
        {
            throw new NotImplementedException("C7A28E4A-70A5-4334-8700-E3FFE44A4604");
        }

        private object DeserializeUsualObject(object obj, Type type, ClassCard card)
        {
#if DEBUG
            _logger.Info($"card = {card}");
#endif

            foreach(var item in card.Fields)
            {
#if DEBUG
                _logger.Info($"item.Item1 (1) = {item.Item1}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("C1177B68-C0AE-4934-9F0A-F9B26330636E", type, item.Item1);
#endif

                ProcessField(obj, item);

                //throw new NotImplementedException("C2BD25AD-4D16-4407-B9C0-B5A1204CADC1");
            }

            foreach (var item in card.Properties)
            {
                var name = item.Item1;

#if DEBUG
                _logger.Info($"name (2) = {name}");
#endif

#if DEBUG
                TmpCheckProcessedMembersOfTypes("C69AE107-BBAF-40EE-838D-02FE94DE613C", type, name);
#endif

                throw new NotImplementedException("C4E71323-8AF9-4149-A2AD-EBCF0B342766");
            }

            throw new NotImplementedException("C0C998E4-4D33-4597-9CED-D51B04036D01");
        }

        private void ProcessField(object obj, (string, int, SerializedValue) item)
        {
            var serializedValue = item.Item3;

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            if (serializedValue.KindOfSerializedValue == KindOfSerializedValue.ExternalValue)
            {
                return;
            }

            var typeId = item.Item2;

#if DEBUG
            _logger.Info($"typeId = {typeId}");
#endif

            var levelType = _serializedTypesPool.GetTypeValue(typeId);

#if DEBUG
            _logger.Info($"levelType.FullName = {levelType.FullName}");
#endif

            var field = levelType.GetField(item.Item1, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

#if DEBUG
            _logger.Info($"field.Name = {field.Name}");
#endif

            var objMember = new ObjMemberRef(obj, field);

            var memberValue = DeserializeValue(serializedValue, objMember);

#if DEBUG
            _logger.Info($"memberValue = {memberValue}");
#endif

            field.SetValue(obj, memberValue);
        }

        private void ProcessProperty(object obj, (string, int, SerializedValue) item)
        {
            var serializedValue = item.Item3;

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            if(serializedValue.KindOfSerializedValue == KindOfSerializedValue.ExternalValue)
            {
                return;
            }

            var typeId = item.Item2;

#if DEBUG
            _logger.Info($"typeId = {typeId}");
#endif

            var levelType = _serializedTypesPool.GetTypeValue(typeId);

#if DEBUG
            _logger.Info($"levelType.FullName = {levelType.FullName}");
#endif

            var property = levelType.GetProperty(item.Item1, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

#if DEBUG
            _logger.Info($"field.Name = {property.Name}");
#endif

            var objMember = new ObjMemberRef(obj, property);

            var memberValue = DeserializeValue(serializedValue, objMember);

#if DEBUG
            _logger.Info($"memberValue = {memberValue}");
#endif

            property.SetValue(obj, memberValue);
        }

        private object DeserializeManualResetEvent(object obj, Type type, ExternalManualResetEventClassCard card)
        {
            throw new NotImplementedException("C5C9CAEE-AAEB-437D-A2CE-552CE3F13FEC");
        }

        private object DeserializeAction(object obj, Type type, ObjMemberRef objMember, ActionCard card)
        {
            throw new NotImplementedException("C7EB3A32-A00E-4AF2-8A09-0E861D749F79");
        }

        private void TmpCheckProcessedTypes(string id, Type type)
        {
            var fullShortTypeName = $"{type.Namespace}.{type.Name}";

#if DEBUG
            //_logger.Info($"type.Name = {type.Name}");
            //_logger.Info($"type.FullName = {type.FullName}");
            //_logger.Info($"type.Namespace = {type.Namespace}");
            //_logger.Info($"fullShortTypeName = {fullShortTypeName}");
#endif

            if (!_tmpProcessedTypes.Contains(type.FullName) && !_tmpProcessedTypes.Contains(fullShortTypeName))
            {
                throw new NotSupportedException($"{id}: please check type '{type.FullName}'");
            }
        }

        private void TmpCheckProcessedMembersOfTypes(string id, Type type, string memberName)
        {
            var fullShortTypeName = $"{type.Namespace}.{type.Name}";

            if (_tmpProcessedMembersOfTypes.TryGetValue(type.FullName, out var memberNamesList) || _tmpProcessedMembersOfTypes.TryGetValue(fullShortTypeName, out memberNamesList))
            {
                if (!memberNamesList.Contains(memberName))
                {
                    throw new NotSupportedException($"{id}: please check member '{memberName}' of type '{type.FullName}'");
                }
            }
            else
            {
                throw new NotSupportedException($"{id}: please check type '{type.FullName}'");
            }
        }

        private List<string> _tmpProcessedTypes { get; set; } = new List<string>() 
        { 
            "SymOntoClay.UnityAsset.Core.World.WorldCore",
            "SymOntoClay.UnityAsset.Core.Internal.WorldContext",
            "SymOntoClay.UnityAsset.Core.WorldSettings"
        };

        private Dictionary<string, List<string>> _tmpProcessedMembersOfTypes { get; set; } = new Dictionary<string, List<string>>();

        private void InitTmpProcessedMembersOfTypes()
        {
            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.World.WorldCore"] = new List<string>() 
            {
                "_context" 
            };

            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.Internal.WorldContext"] = new List<string>() 
            {
                "_isInitialized",
                "_settings"
            };

            _tmpProcessedMembersOfTypes["SymOntoClay.UnityAsset.Core.WorldSettings"] = new List<string>()
            { 
                "LibsDirs",
                "ImagesRootDir",
                "DictionariesDirs",
                "BuiltInStandardLibraryDir"
            };
        }
    }
}
