using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueLinearResolver: BaseResolver
    {
        public LogicalValueLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedLogicalValue Resolve(IndexedValue source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch(sourceKind)
            {
                case KindOfValue.NullValue:
                    if(options.IsDeepMode)
                    {
                        return IndexedValueConvertor.ConvertDeeplyNullValueToLogicalValue(source.AsNullValue, _context.Dictionary);
                    }
                    return IndexedValueConvertor.ConvertFluentlyNullValueToLogicalValue(source.AsNullValue, _context.Dictionary);

                case KindOfValue.LogicalValue:
                    return source.AsLogicalValue;

                case KindOfValue.NumberValue:
                    if (options.IsDeepMode)
                    {
                        return IndexedValueConvertor.ConvertDeeplyNumberValueToLogicalValue(source.AsNumberValue, _context.Dictionary);
                    }
                    return IndexedValueConvertor.ConvertFluentlyNumberValueToLogicalValue(source.AsNumberValue, _context.Dictionary);

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
