using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class NumberValueLinearResolver : BaseResolver
    {
        public NumberValueLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedNumberValue Resolve(IndexedValue source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch (sourceKind)
            {
                case KindOfValue.NullValue:
                    if(options.IsDeepMode)
                    {
                        return IndexedValueConvertor.ConvertDeeplyNullValueToNumberValue(source.AsNullValue, _context.Dictionary);
                    }                    
                    return IndexedValueConvertor.ConvertFluentlyNullValueToNumberValue(source.AsNullValue, _context.Dictionary);

                case KindOfValue.LogicalValue:
                    if (options.IsDeepMode)
                    {
                        return IndexedValueConvertor.ConvertDeeplyLogicalValueToNumberValue(source.AsLogicalValue, _context.Dictionary);
                    }
                    return IndexedValueConvertor.ConvertFluentlyLogicalValueToNumberValue(source.AsLogicalValue, _context.Dictionary);

                case KindOfValue.NumberValue:
                    return source.AsNumberValue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
