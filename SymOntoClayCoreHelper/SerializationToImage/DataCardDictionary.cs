using SymOntoClay.Common.Disposing;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class DataCardDictionary : Disposable
    {
        public DataCardDictionary(string basePath, string packEntryName)
        {
        }

        public DataCardDictionary(string fullFileName)
        {
        }

        private readonly DataCardReader _dataCardReader;
    }
}
