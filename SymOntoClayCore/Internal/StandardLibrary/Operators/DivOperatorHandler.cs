using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class DivOperatorHandler : BaseLoggedComponent, IBinaryOperatorHandler
    {
        public DivOperatorHandler(IEngineContext engineContext)
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

            if (leftOperand.IsNullValue || rightOperand.IsNullValue)
            {
                return new NullValue();
            }

            if (leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                var leftOperandValue = leftOperand.GetSystemValue();
                var rightOperandValue = rightOperand.GetSystemValue();

                var rightOperandNumSysValue = (double)rightOperandValue;

                if(rightOperandNumSysValue == 0)
                {
                    return new NullValue();
                }

                return new NumberValue((double)leftOperandValue / rightOperandNumSysValue);
            }

            throw new NotImplementedException();
        }
    }
}
