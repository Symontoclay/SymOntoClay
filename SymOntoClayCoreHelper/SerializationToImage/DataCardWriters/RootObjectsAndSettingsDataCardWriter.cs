using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class RootObjectsAndSettingsDataCardWriter: BaseDataCardWriter
    {
        public RootObjectsAndSettingsDataCardWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.RootObjectsAndSettings)
        {
        }
    }
}
