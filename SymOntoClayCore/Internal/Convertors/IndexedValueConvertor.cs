using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class IndexedValueConvertor
    {
        public static IndexedLogicalValue ConvertDeeplyNullValueToLogicalValue(IndexedNullValue source, IEntityDictionary entityDictionary)
        {
            throw new NotImplementedException();
        }

        public static IndexedLogicalValue ConvertFluentlyNullValueToLogicalValue(IndexedNullValue source, IEntityDictionary entityDictionary)
        {
            var result = new IndexedLogicalValue();
            result.SystemValue = null;

            return result;
        }

        public static IndexedLogicalValue ConvertDeeplyNumberValueToLogicalValue(IndexedNumberValue source, IEntityDictionary entityDictionary)
        {
            throw new NotImplementedException();
        }

        public static IndexedLogicalValue ConvertFluentlyNumberValueToLogicalValue(IndexedNumberValue source, IEntityDictionary entityDictionary)
        {
            var result = new IndexedLogicalValue();
            if(source.SystemValue.HasValue)
            {
                result.SystemValue = (float)source.SystemValue.Value;
            }

            return result;
        }

        public static IndexedNumberValue ConvertDeeplyNullValueToNumberValue(IndexedNullValue source, IEntityDictionary entityDictionary)
        {
            throw new NotImplementedException();
        }

        public static IndexedNumberValue ConvertFluentlyNullValueToNumberValue(IndexedNullValue source, IEntityDictionary entityDictionary)
        {
            var result = new IndexedNumberValue();

            result.SystemValue = null;

            return result;
        }

        public static IndexedNumberValue ConvertDeeplyLogicalValueToNumberValue(IndexedLogicalValue source, IEntityDictionary entityDictionary)
        {
            throw new NotImplementedException();
        }

        public static IndexedNumberValue ConvertFluentlyLogicalValueToNumberValue(IndexedLogicalValue source, IEntityDictionary entityDictionary)
        {
            var result = new IndexedNumberValue();

            if (source.SystemValue.HasValue)
            {
                result.SystemValue = source.SystemValue.Value;
            }

            return result;
        }
    }
}
