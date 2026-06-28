using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ImageRootCardCardWriter : BaseDataCardWriter
    {
        public ImageRootCardCardWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.ImageRoot)
        {
        }
    }
}
