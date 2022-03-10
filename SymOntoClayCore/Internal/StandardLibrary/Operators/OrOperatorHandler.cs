using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class OrOperatorHandler : BaseLoggedComponent, IBinaryOperatorHandler
    {
        public OrOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
        }

        /// <inheritdoc/>
        public Value Call(Value leftOperand, Value rightOperand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"leftOperand = {leftOperand}");
            Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            throw new NotImplementedException();
        }
    }
}
