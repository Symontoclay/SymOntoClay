using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class StatesResolver : BaseResolver
    {
        public StatesResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public StateDef Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(name, localCodeExecutionContext, _defaultOptions);
        }

        public StateDef Resolve(StrongIdentifierValue name, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"name = {name}");
#endif

            throw new NotImplementedException();
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
