using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class LessOrEqualOperatorHandler : BaseLoggedComponent, IBinaryOperatorHandler
    {
        public LessOrEqualOperatorHandler(IEngineContext engineContext)
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
                var leftOperandValue = leftOperand.AsNumberValue.GetSystemValue();
                var rightOperandValue = rightOperand.AsNumberValue.GetSystemValue();

                if ((double)leftOperandValue <= (double)rightOperandValue)
                {
                    return new LogicalValue(1);
                }

                return new LogicalValue(0);
            }

            throw new NotImplementedException();
        }
    }
}
