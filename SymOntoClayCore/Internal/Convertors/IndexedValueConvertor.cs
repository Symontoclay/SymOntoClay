using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class IndexedValueConvertor
    {
        public static IndexedLogicalValue ConvertNullValueToLogicalValue(IndexedNullValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertNullValueToLogicalValue(source.OriginalNullValue, mainStorageContext).GetIndexed(mainStorageContext);
        }

        public static IndexedLogicalValue ConvertNumberValueToLogicalValue(IndexedNumberValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertNumberValueToLogicalValue(source.OriginalNumberValue, mainStorageContext).GetIndexed(mainStorageContext);
        }

        public static IndexedNumberValue ConvertNullValueToNumberValue(IndexedNullValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertNullValueToNumberValue(source.OriginalNullValue, mainStorageContext).GetIndexed(mainStorageContext);
        }

        public static IndexedNumberValue ConvertLogicalValueToNumberValue(IndexedLogicalValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertLogicalValueToNumberValue(source.OriginalLogicalValue, mainStorageContext).GetIndexed(mainStorageContext);
        }
    }
}
