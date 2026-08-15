using SymOntoClay.CoreHelper.SerializationToImage.DataCardReaders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectDeserialializationFactory: IObjectDeserialializationFactory
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjectDeserialializationFactory(IDataCardDictionary objectsDataCardDictionary, ISerializedTypesPool serializedTypesPool, 
            ISerializedObjectsPool serializedObjectsPool, IDataCardReader deserializedRootObjectsAndSettingsdataCardReader, List<IDataCard> rootObjectsAndSettingsDataCardsList)
        {
            _objectsDataCardDictionary = objectsDataCardDictionary;
            _serializedTypesPool = serializedTypesPool;
            _serializedObjectsPool = serializedObjectsPool;
            _deserializedRootObjectsAndSettingsdataCardReader = deserializedRootObjectsAndSettingsdataCardReader;
            _rootObjectsAndSettingsDataCardsList = rootObjectsAndSettingsDataCardsList;

            PrepareDictionaryForReplace();
        }

        private readonly IDataCardDictionary _objectsDataCardDictionary;
        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly ISerializedObjectsPool _serializedObjectsPool;
        private readonly IDataCardReader _deserializedRootObjectsAndSettingsdataCardReader;
        private readonly List<IDataCard> _rootObjectsAndSettingsDataCardsList;
        private Dictionary<SerializedValue, object> _alreadyExistingObjects = new Dictionary<SerializedValue, object>();

        private void PrepareDictionaryForReplace()
        {
#if DEBUG
            _logger.Info($"_rootObjectsAndSettingsDataCardsList.Count = {_rootObjectsAndSettingsDataCardsList.Count}");
#endif

            var serializedCardsList = _rootObjectsAndSettingsDataCardsList.Cast<IDataCardWithPath>();

#if DEBUG
            _logger.Info($"serializedCardsList.Count() = {serializedCardsList.Count()}");
#endif

            if(!serializedCardsList.Any())
            {
                return;
            }

            var deserializedCardsList = _deserializedRootObjectsAndSettingsdataCardReader.ReadAll().Cast<IDataCardWithPath>();

#if DEBUG
            _logger.Info($"deserializedCardsList.Count() = {deserializedCardsList.Count()}");
#endif

            throw new NotImplementedException("C8F2EC48-9FB7-4F8A-9D9D-D384458EE376");
        }

        /// <inheritdoc/>
        public object GetValue(SerializedValue serializedValue)
        {
#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            if(_alreadyExistingObjects.TryGetValue(serializedValue, out var existingObj))
            {
                return existingObj;
            }

            var type = _serializedTypesPool.GetTypeValue(serializedValue.TypeId);

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
#endif

            throw new NotImplementedException("C5F8FEE4-C8A8-48FC-A23E-CBEBDE5E47EF");
        }
    }
}
