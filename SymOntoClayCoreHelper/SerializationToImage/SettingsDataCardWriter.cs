using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.Common.Disposing;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SettingsDataCardWriter: BaseDataCardWriter
    {
        public SettingsDataCardWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack)
            : base(basePath, filesToPack, PackEntryNames.Settings)
        {
        }
    }
}
