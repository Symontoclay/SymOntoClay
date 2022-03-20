using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class CodeItemDirectivesResolver : BaseResolver
    {
        public CodeItemDirectivesResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public List<CodeItemDirective> Resolve(LocalCodeExecutionContext localCodeExecutionContext)
        {
            return Resolve(localCodeExecutionContext, _defaultOptions);
        }

        public List<CodeItemDirective> Resolve(LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
            throw new NotImplementedException();
        }

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();
    }
}
