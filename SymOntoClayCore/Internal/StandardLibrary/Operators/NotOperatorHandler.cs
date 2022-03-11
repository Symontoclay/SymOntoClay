using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class NotOperatorHandler : BaseLoggedComponent, IUnaryOperatorHandler
    {
        public NotOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
        }

        /// <inheritdoc/>
        public Value Call(Value operand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"operand = {operand}");
            //Log($"annotation = {annotation}");
#endif

            throw new NotImplementedException();
        }
    }
}
