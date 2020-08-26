using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class StrongIdentifierLinearResolver : BaseResolver
    {
        public StrongIdentifierLinearResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedStrongIdentifierValue Resolve(IndexedValue source, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"source = {source}");
#endif

            var sourceKind = source.KindOfValue;

            switch (sourceKind)
            {
                case KindOfValue.StrongIdentifierValue:
                    return source.AsStrongIdentifierValue;

                case KindOfValue.InstanceValue:
                    return source.AsInstanceValue.InstanceInfo.IndexedName;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }
        }
    }
}
