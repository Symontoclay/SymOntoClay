using SymOntoClay.Common.Disposing;
using System;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class DataCardListWriter: Disposable, IDataCardWriter
    {
        /// <inheritdoc/>
        public string RelativePath => string.Empty;

        /// <inheritdoc/>
        public void Write(IDataCard dataCard)
        {
            throw new NotImplementedException("C00B9E99-56CF-430F-B3CE-FB040BDA497D");
        }
    }
}
