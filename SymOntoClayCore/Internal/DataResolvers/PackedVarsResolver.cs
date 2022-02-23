using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PackedVarsResolver: IPackedVarsResolver
    {
        public PackedVarsResolver(VarsResolver varsResolver, LocalCodeExecutionContext localCodeExecutionContext)
            : this(varsResolver, localCodeExecutionContext, varsResolver.DefaultOptions)
        {
        }

        public PackedVarsResolver(VarsResolver varsResolver, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            _varsResolver = varsResolver;
            _localCodeExecutionContext = localCodeExecutionContext;
            _options = options;
        }

        private readonly VarsResolver _varsResolver;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly ResolverOptions _options;

        /// <inheritdoc/>
        public Value GetVarValue(StrongIdentifierValue varName)
        {
            return _varsResolver.GetVarValue(varName, _localCodeExecutionContext, _options);
        }
    }
}
