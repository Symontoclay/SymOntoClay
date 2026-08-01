using SymOntoClay.Common.Disposing;
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
        public IDataCardWithHeader GetDataCardByHeader(SerializedValue header)
        {
            return _cardsDict[header];
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
