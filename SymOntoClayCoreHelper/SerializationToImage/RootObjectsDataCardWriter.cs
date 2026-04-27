using SymOntoClay.Common.Disposing;
using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class RootObjectsDataCardWriter : BaseDataCardWriter
    {
        public RootObjectsDataCardWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.RootObjects)
        {
        }
    }
}
