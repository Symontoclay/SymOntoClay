using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using SymOntoClay.CoreHelper.SerializationToImage.ComponentsInterfaces;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class RootObjectsSerializer : BaseObjectSerializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public RootObjectsSerializer(ISerializedObjectsPool serializedObjectsPool, ISerializedTypesPool serializedTypesPool, IStructuralContext structuralContext, IDataCardWriter dataCardWriter)
            : base(serializedObjectsPool, serializedTypesPool)
        {
            _structuralContext = structuralContext;
            _dataCardWriter = dataCardWriter;
        }

        private readonly IStructuralContext _structuralContext;
        private readonly IDataCardWriter _dataCardWriter;

        /// <inheritdoc/>
        protected override SerializedValue SerializeReflectionType(object obj, Type type)
        {
            throw new NotImplementedException("C707B938-0FA8-4AD4-98D8-C5A805EB1011");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeBareObject(object obj, Type type, string path)
        {
            throw new NotImplementedException("C8C98387-18AF-46E4-8F64-80EC18F28503");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeArray(object obj, Type type, string path)
        {
            throw new NotImplementedException("9300E862-DEFB-4EA8-9EA8-D03DAB56B887");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericList(object obj, Type type, string path)
        {
            throw new NotImplementedException("C05F3042-8B65-41EC-9834-144108D9AF95");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj, Type type, string path)
        {
            throw new NotImplementedException("C591FCD9-6D2F-42E8-AB57-2A57FFDCA255");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
            throw new NotImplementedException("C8215DCD-CE51-4E00-AE0B-4D8D4706739D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeHashSet(object obj, Type type, string path)
        {
            throw new NotImplementedException("C0C15C0A-3F3D-4354-B3CA-303483AA0185");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
            throw new NotImplementedException("C88F72B2-982F-4958-B7B9-79E96279297E");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj, Type type, string path)
        {
#if DEBUG
            TmpCheckProcessedTypes("3862A9A0-0E28-4A26-AD77-9788DD2C9C3F", type);
#endif

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            //_logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
#endif

            switch (kindOfStructuralObject)
            {
                case KindOfStructuralObject.WorldRoot:
                    return SerializeWorldRoot(obj, type, path);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfStructuralObject), kindOfStructuralObject, "BE319AC6-DFCC-4225-8C10-206E4626D019");
            }

            throw new NotImplementedException("C7A14DEA-D4C7-4C42-A011-91B57D760894");
        }

        private SerializedValue SerializeWorldRoot(object obj, Type type, string path)
        {
#if DEBUG
            //_logger.Info($"path = {path}");
#endif

            var world = (ISerializedWorldRoot)obj;

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.IsPreregistered);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalWorldRootClassCard()
            {
                Header = serializedValue
            };

            var cardItems = new List<SerializedValue>();

            foreach(var worldComponent in world.SerializedWorldComponents)
            {
                var fieldSerializedValue = SerializeWorldComponent(worldComponent);

#if DEBUG
                //_logger.Info($"fieldSerializedValue = {fieldSerializedValue}");
#endif

                cardItems.Add(fieldSerializedValue);
            }

            card.Items = cardItems;

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        private SerializedValue SerializeWorldComponent(ISerializedWorldComponent obj)
        {
            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.IsPreregistered);

#if DEBUG
            //_logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalWorldComponentClassCard()
            {
                Header = serializedValue,
                Id = obj.Id
            };

#if DEBUG
            //_logger.Info($"card = {card}");
#endif

            _dataCardWriter.Write(card);

            return serializedValue;
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeManualResetEvent(object obj, Type type, string path)
        {
            throw new NotImplementedException("C3AED0A8-0AE1-4CC0-B80A-C1C3AF629DE6");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj, Type type)
        {
            throw new NotImplementedException("C140367A-7BC2-4A3A-8BBF-A059D907E7E0");
        }

#if DEBUG
        /// <inheritdoc/>
        protected override List<string> _tmpProcessedTypes { get; set; } = new List<string>()
        {
            "SymOntoClay.UnityAsset.Core.World.WorldCore"
        };

        /// <inheritdoc/>
        protected override Dictionary<string, List<string>> _tmpProcessedMembersOfTypes { get; set; } = new Dictionary<string, List<string>>();
#endif
    }
}
