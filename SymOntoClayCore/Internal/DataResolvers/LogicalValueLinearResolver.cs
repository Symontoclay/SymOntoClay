using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueLinearResolver: BaseResolver
    {
        public LogicalValueLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public LogicalValue Resolve(Value source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch (sourceKind)
            {
                case KindOfValue.NullValue:
                    return new LogicalValue(null);

                case KindOfValue.LogicalValue:
                    return source.AsLogicalValue;

                case KindOfValue.NumberValue:
                    return new LogicalValue((float?)source.AsNumberValue.SystemValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }

        public IndexedLogicalValue Resolve(IndexedValue source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch(sourceKind)
            {
                case KindOfValue.NullValue:
                    return IndexedValueConvertor.ConvertNullValueToLogicalValue(source.AsNullValue, _context);

                case KindOfValue.LogicalValue:
                    return source.AsLogicalValue;

                case KindOfValue.NumberValue:
                    return IndexedValueConvertor.ConvertNumberValueToLogicalValue(source.AsNumberValue, _context);

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
