using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class RootObjectsAndSettingsDataCardFileWriter: BaseDataCardFileWriter
    {
        public RootObjectsAndSettingsDataCardFileWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.RootObjectsAndSettings)
        {
        }
    }
}
