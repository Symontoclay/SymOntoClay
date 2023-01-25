using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class MetadataResolver : BaseResolver
    {
        public MetadataResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public CodeItem Resolve(StrongIdentifierValue prototypeName, LocalCodeExecutionContext executionContext)
        {
            return Resolve(prototypeName, executionContext, _defaultOptions);
        }

        public CodeItem Resolve(StrongIdentifierValue prototypeName, LocalCodeExecutionContext executionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"prototypeName = {prototypeName}");
#endif

            throw new NotImplementedException();
        }

        /*
         var metadataStorage = globalStorage.MetadataStorage;
        var codeItem = metadataStorage.GetByName(weightedInheritanceItem.SuperName);
         */

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
