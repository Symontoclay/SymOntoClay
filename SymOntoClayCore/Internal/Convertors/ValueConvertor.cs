using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class ValueConvertor
    {
        public static LogicalValue ConvertNullValueToLogicalValue(NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue(null);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static LogicalValue ConvertNumberValueToLogicalValue(NumberValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue((float?)source.AsNumberValue.SystemValue);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static NumberValue ConvertNullValueToNumberValue(NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(null);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static NumberValue ConvertLogicalValueToNumberValue(LogicalValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(source.AsLogicalValue.SystemValue);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(AnnotatedItem source, AnnotatedItem dest, IMainStorageContext mainStorageContext)
        {
            throw new NotImplementedException();
        }
    }
}
