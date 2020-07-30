using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class IsOperatorHandler: BaseLoggedComponent, IBinaryOperatorHandler
    {
        public IsOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
            _inheritanceResolver = engineContext.DataResolversFactory.GetInheritanceResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly InheritanceResolver _inheritanceResolver;

        /// <inheritdoc/>
        public IndexedValue Call(IndexedValue leftOperand, IndexedValue rightOperand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            if (leftOperand.IsStrongIdentifierValue && leftOperand.IsStrongIdentifierValue)
            {
                return GetInheritanceRank(leftOperand, rightOperand, localCodeExecutionContext);
            }

            throw new NotImplementedException();
        }

        private IndexedValue GetInheritanceRank(IndexedValue leftOperand, IndexedValue rightOperand, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return _inheritanceResolver.GetInheritanceRank(leftOperand.AsStrongIdentifierValue, rightOperand.AsStrongIdentifierValue, localCodeExecutionContext, ResolverOptions.GetDefaultOptions());
        }
    }
}
