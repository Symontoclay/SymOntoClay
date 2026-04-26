using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public readonly record struct DataCardRecord(
        int KindOfDataCard,
        int DataLength,
        byte[] Data
        );
}
