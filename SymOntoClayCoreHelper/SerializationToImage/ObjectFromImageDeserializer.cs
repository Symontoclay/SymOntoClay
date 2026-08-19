using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectFromImageDeserializer: IObjectDeserializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjectFromImageDeserializer(IObjectDeserialializationFactory objectDeserialializationFactory, IDataCardDictionary objectsDataCardDictionary)
        {
            _objectDeserialializationFactory = objectDeserialializationFactory;
            _objectsDataCardDictionary = objectsDataCardDictionary;
        }

        private readonly IObjectDeserialializationFactory _objectDeserialializationFactory;
        private readonly IDataCardDictionary _objectsDataCardDictionary;

        /// <inheritdoc/>
        public object DeserializeValue(SerializedValue serializedValue)
        {
#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var obj = _objectDeserialializationFactory.GetValue(serializedValue);

#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var dataCard = _objectsDataCardDictionary.GetDataCardByHeader(serializedValue);

#if DEBUG
            _logger.Info($"dataCard = {dataCard}");
#endif

            throw new NotImplementedException("C9BA50F3-73ED-454D-B5E0-90CD3A73E805");
        }
    }
}
