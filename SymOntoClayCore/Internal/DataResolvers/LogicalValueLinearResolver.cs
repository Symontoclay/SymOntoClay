using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalValueLinearResolver: BaseResolver
    {
        public LogicalValueLinearResolver(IEngineContext context)
            : base(context)
        {
        }

        public LogicalValue Resolve(Value source, IStorage storage)
        {
#if DEBUG
            Log($"source = {source}");
#endif

            var sourceKind = source.Kind;

            switch(sourceKind)
            {
                case KindOfValue.LogicalValue:
                    return source.AsLogicalValue;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
