using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PackedLogicalValueModalityResolver: IPackedLogicalValueModalityResolver
    {
        public PackedLogicalValueModalityResolver(LogicalValueModalityResolver logicalValueModalityResolver, LocalCodeExecutionContext localCodeExecutionContext)
        {
            _logicalValueModalityResolver = logicalValueModalityResolver;
            _localCodeExecutionContext = localCodeExecutionContext;
        }

        private readonly LogicalValueModalityResolver _logicalValueModalityResolver;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;

        /// <inheritdoc/>
        public bool IsHigh(Value modalityValue)
        {
            return _logicalValueModalityResolver.IsHigh(modalityValue, _localCodeExecutionContext);
        }
    }
}
