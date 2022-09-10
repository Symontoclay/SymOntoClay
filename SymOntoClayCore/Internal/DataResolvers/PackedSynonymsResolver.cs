using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PackedSynonymsResolver: IPackedSynonymsResolver
    {
        public PackedSynonymsResolver(SynonymsResolver synonymsResolver, LocalCodeExecutionContext localCodeExecutionContext)
            : this(synonymsResolver, localCodeExecutionContext, synonymsResolver.DefaultOptions)
        {
        }

        public PackedSynonymsResolver(SynonymsResolver synonymsResolver, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            _synonymsResolver = synonymsResolver;
            _localCodeExecutionContext = localCodeExecutionContext;
            _options = options;
        }

        private readonly SynonymsResolver _synonymsResolver;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly ResolverOptions _options;

        /// <inheritdoc/>
        public List<StrongIdentifierValue> GetSynonyms(StrongIdentifierValue name)
        {
            return _synonymsResolver.GetSynonyms(name, _localCodeExecutionContext);
        }
    }
}
