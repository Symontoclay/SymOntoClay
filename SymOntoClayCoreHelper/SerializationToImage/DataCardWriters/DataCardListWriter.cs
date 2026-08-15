using SymOntoClay.Common.Disposing;
using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class DataCardListWriter: Disposable, IDataCardWriter
    {
        public DataCardListWriter(List<IDataCard> dataCardsList) 
        {
            _dataCardsList = dataCardsList;
        }

        private readonly List<IDataCard> _dataCardsList;

        /// <inheritdoc/>
        public string RelativePath => string.Empty;

        /// <inheritdoc/>
        public void Write(IDataCard dataCard)
        {
            _dataCardsList.Add(dataCard);
        }
    }
}
