using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectFromImageDeserializer: IObjectDeserializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjectFromImageDeserializer(IObjectDeserialializationFactory objectDeserialializationFactory)
        {
            _objectDeserialializationFactory = objectDeserialializationFactory;
        }

        private readonly IObjectDeserialializationFactory _objectDeserialializationFactory;

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

            throw new NotImplementedException("C9BA50F3-73ED-454D-B5E0-90CD3A73E805");
        }
    }
}
