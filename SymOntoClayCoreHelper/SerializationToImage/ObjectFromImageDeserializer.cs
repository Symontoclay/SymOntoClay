using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectFromImageDeserializer: IObjectDeserializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjectFromImageDeserializer(IObjectDeserialializationFactory objectDeserialializationFactory, IDataCardDictionary objectsDataCardDictionary, IStructuralContext structuralContext)
        {
            _objectDeserialializationFactory = objectDeserialializationFactory;
            _objectsDataCardDictionary = objectsDataCardDictionary;
            _structuralContext = structuralContext;
        }

        private readonly IObjectDeserialializationFactory _objectDeserialializationFactory;
        private readonly IDataCardDictionary _objectsDataCardDictionary;
        private readonly IStructuralContext _structuralContext;

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

            if(obj == null)
            {
                return null;
            }

            var dataCard = _objectsDataCardDictionary.GetDataCardByHeader(serializedValue);

#if DEBUG
            _logger.Info($"dataCard = {dataCard}");
#endif

            var type = obj.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
            _logger.Info($"type.IsArray = {type.IsArray}");
#endif

            if (type.IsEnum)
            {
                return DeserializePrimitiveType(obj, type);
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
                    return DeserializePrimitiveType(obj, type);

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

        private object DeserializePrimitiveType(object obj, Type type)
        {
            throw new NotFiniteNumberException("C6466865-0D32-4523-9963-2D2244827666");
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

            throw new NotImplementedException("C22099BC-728D-4059-9D80-5FA16DDC433D");
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

        private List<string> _tmpProcessedTypes { get; set; } = new List<string>() 
        { 
            "SymOntoClay.UnityAsset.Core.World.WorldCore" 
        };
    }
}
