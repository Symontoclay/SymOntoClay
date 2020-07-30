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
            //Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch (sourceKind)
            {
                case KindOfValue.NullValue:
                    return IndexedValueConvertor.ConvertNullValueToNumberValue(source.AsNullValue, _context);

                case KindOfValue.LogicalValue:
                    return IndexedValueConvertor.ConvertLogicalValueToNumberValue(source.AsLogicalValue, _context);

                case KindOfValue.NumberValue:
                    return source.AsNumberValue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
