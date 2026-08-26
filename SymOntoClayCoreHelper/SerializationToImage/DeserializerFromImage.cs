using Newtonsoft.Json;
using SymOntoClay.CoreHelper.SerializationToImage.DataCardReaders;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters;
using SymOntoClay.CoreHelper.SerializationToImage.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class DeserializerFromImage
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public DeserializerFromImage(SerializationToImageSettings serializationSettings, IStructuralContext structuralContext)
        {
            _structuralContext = structuralContext;

            _imageFileName = serializationSettings.ImageFileName;

            _baseTempPath = serializationSettings.BaseTempPath;

            if (string.IsNullOrWhiteSpace(_baseTempPath))
            {
                _baseTempPath = Environment.GetEnvironmentVariable("TMP");
            }

#if DEBUG
            //_logger.Info($"_baseTempPath = {_baseTempPath}");
#endif

            _tempPath = Path.Combine(_baseTempPath, $"TempImage_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }

            _serializedTypesPool = new SerializedTypesPool();
            _typesHelper = new TypesHelper();
        }

        private readonly IStructuralContext _structuralContext;
        private readonly string _imageFileName;
        private readonly string _baseTempPath;
        private readonly string _tempPath;
        private ImageManifest _manifest;
        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly ITypesHelper _typesHelper;
        private SerializedValue _rootSerializedValue;
        private IDataCardDictionary _objectsDataCardDictionary;
        private IDataCardReader _deserializedRootObjectsAndSettingsdataCardReader;
        private IObjectDeserialializationFactory _objectDeserialializationFactory;
        private IObjectDeserializer _objectDeserializer;

        private ISerializedObjectsPool _serializedObjectsPool;
        private List<IDataCard> _rootObjectsDataCardsList = new List<IDataCard>();
        private List<IDataCard> _rootObjectsAndSettingsDataCardsList = new List<IDataCard>();
        private IDataCardWriter _rootObjectsDataCardWriter;
        private IDataCardWriter _rootObjectsAndSettingsDataCardWriter;
        private IObjectSerializer _rootObjectsSerializer;
        private IObjectSerializer _rootObjectsAndSettingsSerializer;

        public void Deserialize(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            CreateCommonObjects();

            CheckExisitgRootObject(obj);

            Preparation();

            var deserializedObj = _objectDeserializer.DeserializeValue(_rootSerializedValue);

#if DEBUG
            _logger.Info($"deserializedObj = {deserializedObj}");
#endif

            Finalization();

            throw new NotImplementedException("C072330D-6772-4582-948D-CA412DCD07FE");
        }

        private void CreateCommonObjects()
        {
            _serializedObjectsPool = new SerializedObjectsPool(_serializedTypesPool, _typesHelper);

            _rootObjectsDataCardWriter = new DataCardListWriter(_rootObjectsDataCardsList);
            _rootObjectsAndSettingsDataCardWriter = new DataCardListWriter(_rootObjectsAndSettingsDataCardsList);

            _rootObjectsSerializer = new RootObjectsSerializer(_serializedObjectsPool, _serializedTypesPool, _structuralContext, _rootObjectsDataCardWriter);
            _rootObjectsAndSettingsSerializer = new RootObjectsAndSettingsSerializer(_serializedObjectsPool, _serializedTypesPool, _structuralContext, _rootObjectsAndSettingsDataCardWriter);
        }

        private void CheckExisitgRootObject(object obj)
        {
            var rootSerializedValue = _rootObjectsSerializer.SerializeValue(obj);

#if DEBUG
            _logger.Info($"rootSerializedValue = {rootSerializedValue}");
            _logger.Info($"_rootObjectsDataCardsList.Count = {_rootObjectsDataCardsList.Count}");
#endif

            var rootSerializedSettingsValue = _rootObjectsAndSettingsSerializer.SerializeValue(obj, "@");

#if DEBUG
            _logger.Info($"rootSerializedSettingsValue = {rootSerializedSettingsValue}");
            _logger.Info($"_rootObjectsAndSettingsDataCardsList.Count = {_rootObjectsAndSettingsDataCardsList.Count}");
#endif
        }

        private void Preparation()
        {
            UnPackPackage();
            ReadManifest();
            LoadSerializedTypesPoolFromFile();
            ReadAndCheckImageRootCard();
            ReadObjectsSerializedValues();
        }

        private void UnPackPackage()
        {
#if DEBUG
            _logger.Info($"_imageFileName = {_imageFileName}");
            _logger.Info($"_tempPath = {_tempPath}");
#endif

            ZipFile.ExtractToDirectory(_imageFileName, _tempPath);
        }

        private void ReadManifest()
        {
            var packEntryName = PackEntryNames.Manifest;

            var fullFileName = Path.Combine(_tempPath, packEntryName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            var content = File.ReadAllText(fullFileName);

#if DEBUG
            _logger.Info($"content = {content}");
#endif

            _manifest = JsonConvert.DeserializeObject<ImageManifest>(content);

#if DEBUG
            _logger.Info($"_manifest = {_manifest}");
#endif
        }

        private void LoadSerializedTypesPoolFromFile()
        {
            var packEntryName = PackEntryNames.Types;

            var fullFileName = Path.Combine(_tempPath, packEntryName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            using var fs = new FileStream(fullFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            using var reader = new BinaryReader(fs);

            _serializedTypesPool.Load(reader);
        }

        private void ReadAndCheckImageRootCard()
        {
            using var imageRootCardDataCardReader = new DataCardReader(_tempPath, _manifest.ImageRootRelativePath);

            var cardsList = imageRootCardDataCardReader.ReadAll();

#if DEBUG
            _logger.Info($"cardsList.Count = {cardsList.Count}");
#endif

            if (cardsList.Count != 1)
            {
                throw new NotImplementedException($"D2E50FC7-B470-440D-A383-C82104E7E71C: cardsList.Count = {cardsList.Count}");
            }

            var imageRootCard = cardsList.Where(p => p.KindOfDataCard == KindOfDataCard.ImageRootCard).Cast<ImageRootCard>();

#if DEBUG
            _logger.Info($"imageRootCard.Count() = {imageRootCard.Count()}");
#endif

            if(imageRootCard.Count() != 1)
            {
                throw new NotImplementedException($"183EB669-243C-44B1-89D4-B59F373A1735: imageRootCard.Count() = {imageRootCard.Count()}");
            }

            var card = imageRootCard.Single();

#if DEBUG
            _logger.Info($"card = {card}");
#endif

            _rootSerializedValue = card.RootSerializedValue;

            if(_rootSerializedValue == null)
            {
                throw new NullReferenceException("6E0AB56C-6F5E-4FD2-9CCB-F8AB4564C1AB");
            }

            if(_rootSerializedValue != card.MainRootSerializedValue)
            {
                throw new NotImplementedException("D2FDB2FC-70EA-4E00-9903-701AC2F80DF9");
            }

            if (_rootSerializedValue != card.RootSerializedSettingsValue)
            {
                throw new NotImplementedException("84F21A9C-F172-4C50-A572-FD2BDDF17DD9");
            }

#if DEBUG
            _logger.Info($"_rootSerializedValue = {_rootSerializedValue}");
#endif
        }

        private void ReadObjectsSerializedValues()
        {
            //NTmpFindRootObject(_manifest.RootObjectsRelativePath);
            //NTmpFindRootObject(_manifest.RootObjectsAndSettingsRelativePath);
            //NTmpFindRootObject(_manifest.ObjectsDataRelativePath);

            _objectsDataCardDictionary = new DataCardDictionary(_tempPath, _manifest.ObjectsDataRelativePath);

#if DEBUG
            _logger.Info($"_objectsDataCardDictionary.GetDataCardByHeader(_rootSerializedValue) = {_objectsDataCardDictionary.GetDataCardByHeader(_rootSerializedValue)}");
#endif

            _deserializedRootObjectsAndSettingsdataCardReader = new DataCardReader(_tempPath, _manifest.RootObjectsAndSettingsRelativePath);

            _objectDeserialializationFactory = new ObjectDeserialializationFactory(_objectsDataCardDictionary, _serializedTypesPool, _serializedObjectsPool, _deserializedRootObjectsAndSettingsdataCardReader, _rootObjectsAndSettingsDataCardsList);

            _objectDeserializer = new ObjectFromImageDeserializer(_objectDeserialializationFactory, _objectsDataCardDictionary, _structuralContext, _serializedTypesPool);
        }

        private void NTmpFindRootObject(string relativePath)
        {
#if DEBUG
            _logger.Info($"_tempPath = {_tempPath}");
            _logger.Info($"relativePath = {relativePath}");

            var fullFileName = Path.Combine(_tempPath, relativePath);

            _logger.Info($"fullFileName = {fullFileName}");

            using var dataCardReader = new DataCardReader(fullFileName);

            var cards = dataCardReader.ReadAll().Cast<IDataCardWithHeader>();

            _logger.Info($"cards.Count() = {cards.Count()}");
            _logger.Info($"_rootSerializedValue = {_rootSerializedValue}");

            var targetObj = cards.FirstOrDefault(p => p.Header.KindOfSerializedValue == _rootSerializedValue.KindOfSerializedValue &&
                p.Header.Id == _rootSerializedValue.Id &&
                p.Header.TypeId == _rootSerializedValue.TypeId &&
                p.Header.Literal == _rootSerializedValue.Literal);

            _logger.Info($"targetObj = {targetObj}");
#endif
        }

        private void Finalization()
        {
            //_dataCardDictionary.Dispose();
        }
    }
}
