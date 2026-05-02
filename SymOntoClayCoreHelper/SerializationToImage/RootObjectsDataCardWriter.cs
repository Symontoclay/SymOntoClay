using System;
using System.Collections.Generic;
using System.Text;

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
