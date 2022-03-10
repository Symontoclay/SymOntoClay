using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class AddOperatorHandler : BaseLoggedComponent, IBinaryOperatorHandler
    {
        public AddOperatorHandler(IEngineContext engineContext)
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
                var leftOperandValue = leftOperand.AsNumberValue.GetSystemValue();
                var rightOperandValue = rightOperand.AsNumberValue.GetSystemValue();

                if (leftOperandValue == null || rightOperandValue == null)
                {
                    return new NullValue();
                }

                return new NumberValue((double)leftOperandValue + (double)rightOperandValue);
            }

            if(leftOperand.IsStringValue || rightOperand.IsStringValue)
            {
                return new StringValue(leftOperand.ToSystemString() + rightOperand.ToSystemString());
            }

            throw new NotImplementedException();
        }
    }
}
