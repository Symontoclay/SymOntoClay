using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class RootObjectsDataCardFileWriter : BaseDataCardFileWriter
    {
        public RootObjectsDataCardFileWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.RootObjects)
        {
        }
    }
}
