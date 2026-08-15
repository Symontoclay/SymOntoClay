using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class ObjectsDataCardWriter : BaseDataCardWriter
    {
        public ObjectsDataCardWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.Objects)
        {
        }
    }
}
