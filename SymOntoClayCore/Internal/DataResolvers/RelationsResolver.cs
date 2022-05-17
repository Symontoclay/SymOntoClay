using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class RelationsResolver : BaseResolver
    {
        public RelationsResolver(IMainStorageContext context)
            : base(context)
        {
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;

        public RelationDescription GetRelation(StrongIdentifierValue name, int paramsCount, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return GetRelation(name, paramsCount, localCodeExecutionContext, DefaultOptions);
        }        

        public RelationDescription GetRelation(StrongIdentifierValue name, int paramsCount, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Log($"name = {name}");
            Log($"paramsCount = {paramsCount}");
#endif

            throw new NotImplementedException();
        }

        public readonly ResolverOptions DefaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
