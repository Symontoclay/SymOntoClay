using SymOntoClay.Common.Disposing;
using SymOntoClay.CoreHelper.SerializationToImage.DataCardReaders;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class DataCardDictionary : Disposable, IDataCardDictionary
    {
        public DataCardDictionary(string basePath, string packEntryName)
        {
            _dataCardReader = new DataCardReader(basePath, packEntryName);

            Init();
        }

        public DataCardDictionary(string fullFileName)
        {
            _dataCardReader = new DataCardReader(fullFileName);

            Init();
        }
        
        private void Init()
        {
            var cards = _dataCardReader.ReadAll().Cast<IDataCardWithHeader>();

            _cardsDict = cards.ToDictionary(p => p.Header, p => p);
        }
        
        private readonly DataCardReader _dataCardReader;

        private Dictionary<SerializedValue, IDataCardWithHeader> _cardsDict;

        /// <inheritdoc/>
        public bool TryGetDataCardByHeader(SerializedValue header, out IDataCardWithHeader card)
        {
            return _cardsDict.TryGetValue(header, out card);
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            _dataCardReader.Dispose();

            _cardsDict.Clear();

            base.OnDisposing();
        }
    }
}
