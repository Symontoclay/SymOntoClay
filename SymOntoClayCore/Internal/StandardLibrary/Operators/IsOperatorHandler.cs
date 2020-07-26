using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
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
        public Value Call(Value leftOperand, Value rightOperand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"leftOperand = {leftOperand}");
            Log($"rightOperand = {rightOperand}");
            Log($"annotation = {annotation}");
#endif

            if (leftOperand.IsIdentifierValue && leftOperand.IsIdentifierValue)
            {
                return GetInheritanceRank(leftOperand, rightOperand, localCodeExecutionContext);
            }

            throw new NotImplementedException();
        }

        private Value GetInheritanceRank(Value leftOperand, Value rightOperand, LocalCodeExecutionContext localCodeExecutionContext)
        {
            return _inheritanceResolver.GetInheritanceRank(leftOperand.AsIdentifierValue, rightOperand.AsIdentifierValue, localCodeExecutionContext, ResolverOptions.GetDefaultFluentOptions());
        }
    }
}
