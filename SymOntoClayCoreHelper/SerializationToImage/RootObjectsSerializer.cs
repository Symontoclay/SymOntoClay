using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class RootObjectsSerializer : BaseObjectSerializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public RootObjectsSerializer(ISerializedObjectsPool serializedObjectsPool, IStructuralContext structuralContext, IDataCardWriter dataCardWriter)
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
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"5E97A06A-EC7B-4CA0-8148-32C63EADB66D: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C8C98387-18AF-46E4-8F64-80EC18F28503");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericList(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"71D06895-4EA4-487D-AEAA-2ABE540BFF85: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C05F3042-8B65-41EC-9834-144108D9AF95");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"E5C73B7A-579A-4717-B59D-81E76B55A038: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C591FCD9-6D2F-42E8-AB57-2A57FFDCA255");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"7FD9BCCA-EF55-412B-8005-C448D15ECC57: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C8215DCD-CE51-4E00-AE0B-4D8D4706739D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"E94DAA30-6BB9-4CAD-B287-95211E714E04: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C88F72B2-982F-4958-B7B9-79E96279297E");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"B1473B52-7534-4DE8-BA1A-FB9A40CBD28E: please check type '{type.FullName}'");
            }
#endif

            var kindOfStructuralObject = _structuralContext.GetKindOfStructuralObject(type);

#if DEBUG
            _logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
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
            _logger.Info($"path = {path}");
#endif

            var serializedValue = _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.IsPreregistered);

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var card = new ExternalClassCard()
            {
                Header = serializedValue,
                Path = path
            };

            var cardFieldDict = new Dictionary<string, SerializedValue>();

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            throw new NotImplementedException("C16052AB-C4CD-4111-AC89-94FBDBFEE487");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeManualResetEvent(object obj, Type type, string path)
        {
            throw new NotImplementedException("C3AED0A8-0AE1-4CC0-B80A-C1C3AF629DE6");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj, Type type)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"F867C305-05AA-4601-B8ED-C20F03BAA377: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C140367A-7BC2-4A3A-8BBF-A059D907E7E0");
        }

        private readonly List<string> _tmpProcessedTypes = new List<string>()
        {
            "SymOntoClay.UnityAsset.Core.World.WorldCore"
        };
    }
}
