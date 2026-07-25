using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectDeserialializationFactory: IObjectDeserialializationFactory
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjectDeserialializationFactory(IDataCardDictionary objectsDataCardDictionary, ISerializedTypesPool serializedTypesPool)
        {
            _objectsDataCardDictionary = objectsDataCardDictionary;
            _serializedTypesPool = serializedTypesPool;
        }

        private readonly IDataCardDictionary _objectsDataCardDictionary;
        private readonly ISerializedTypesPool _serializedTypesPool;

        /// <inheritdoc/>
        public object GetValue(SerializedValue serializedValue)
        {
#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            throw new NotImplementedException("C5F8FEE4-C8A8-48FC-A23E-CBEBDE5E47EF");
        }
    }
}
