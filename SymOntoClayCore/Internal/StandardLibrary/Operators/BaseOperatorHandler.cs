using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public abstract class BaseOperatorHandler: BaseLoggedComponent
    {
        protected BaseOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            var dataResolversFactory = engineContext.DataResolversFactory;

            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        private readonly VarsResolver _varsResolver;

        protected Value TryResolveFromVar(Value operand, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return ValueResolvingHelper.TryResolveFromVar(operand, localCodeExecutionContext, _varsResolver);
        }
    }
}
