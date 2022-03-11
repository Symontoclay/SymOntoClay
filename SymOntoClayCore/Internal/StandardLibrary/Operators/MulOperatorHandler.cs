using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class MulOperatorHandler : BaseLoggedComponent, IBinaryOperatorHandler
    {
        public MulOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
        }

        /// <inheritdoc/>
        public Value Call(Value leftOperand, Value rightOperand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            if (leftOperand.IsSystemNull || rightOperand.IsSystemNull)
            {
                return new NullValue();
            }

            if (leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                return new NumberValue((double)leftOperand.GetSystemValue() * (double)rightOperand.GetSystemValue());
            }

            throw new NotImplementedException();
        }
    }
}
