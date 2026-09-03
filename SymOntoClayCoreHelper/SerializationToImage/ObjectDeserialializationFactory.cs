using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
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
            //_logger.Info($"_rootObjectsAndSettingsDataCardsList.Count = {_rootObjectsAndSettingsDataCardsList.Count}");
#endif

            var serializedCardsList = _rootObjectsAndSettingsDataCardsList.Cast<IDataCardWithPath>().Where(p => p.ShouldBeReplacedDuringDeserialization);

#if DEBUG
            //_logger.Info($"serializedCardsList.Count() = {serializedCardsList.Count()}");
#endif

            if(!serializedCardsList.Any())
            {
                return;
            }

            var deserializedCardsList = _deserializedRootObjectsAndSettingsdataCardReader.ReadAll().Cast<IDataCardWithPath>().Where(p => p.ShouldBeReplacedDuringDeserialization);

#if DEBUG
            //_logger.Info($"deserializedCardsList.Count() = {deserializedCardsList.Count()}");
#endif

            var serializedCardsDict = serializedCardsList.ToDictionary(p => p.Path, p => p.Header);
            var deserializedCardsDict = deserializedCardsList.ToDictionary(p => p.Path, p => p.Header);

            foreach (var item in deserializedCardsDict)
            {
                var path = item.Key;
                var serializedValue = item.Value;

#if DEBUG
                _logger.Info($"path = {path}");
                _logger.Info($"serializedValue = {serializedValue}");
#endif

                if(serializedCardsDict.ContainsKey(path))
                {
                    if(_serializedObjectsPool.TryGetObject(serializedValue, out var obj))
                    {
#if DEBUG
                        _logger.Info($"obj?.GetType()?.FullName = {obj?.GetType()?.FullName}");
#endif

                        _alreadyExistingObjects[serializedValue] = obj;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public object GetValue(SerializedValue serializedValue)
        {
#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            if(_alreadyExistingObjects.TryGetValue(serializedValue, out var existingObj))
            {
#if DEBUG
                _logger.Info($"return existingObj");
#endif

                return existingObj;
            }

            if(serializedValue.TypeId == _serializedTypesPool.NullTypeId)
            {
                return null;
            }

            var type = _serializedTypesPool.GetTypeValue(serializedValue.TypeId);

#if DEBUG
            _logger.Info($"type?.FullName = {type?.FullName}");
#endif

            if(type.FullName == "System.String")
            {
                return serializedValue.Literal;
            }

            if (type.IsDefined(typeof(SerializeWithDataCreationAttribute), true))
            {
                return SerializeWithDataCreationStub.Instance;
            }

            var instance = Activator.CreateInstance(type, nonPublic: true);

            _alreadyExistingObjects[serializedValue] = instance;

            return instance;
        }
    }
}
