using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class PackedRelationsResolver: IPackedRelationsResolver
    {
        public PackedRelationsResolver(RelationsResolver relationsResolver, LocalCodeExecutionContext localCodeExecutionContext)
            : this(relationsResolver, localCodeExecutionContext, relationsResolver.DefaultOptions)
        {
        }

        public PackedRelationsResolver(RelationsResolver relationsResolver, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            _relationsResolver = relationsResolver;
            _localCodeExecutionContext = localCodeExecutionContext;
            _options = options;
        }

        private readonly RelationsResolver _relationsResolver;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly ResolverOptions _options;

        /// <inheritdoc/>
        public RelationDescription GetRelation(StrongIdentifierValue name, int paramsCount)
        {
            return _relationsResolver.GetRelation(name, paramsCount, _localCodeExecutionContext, _options);
        }
    }
}
