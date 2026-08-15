using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class ObjectsDataCardFileWriter : BaseDataCardFileWriter
    {
        public ObjectsDataCardFileWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.Objects)
        {
        }
    }
}
