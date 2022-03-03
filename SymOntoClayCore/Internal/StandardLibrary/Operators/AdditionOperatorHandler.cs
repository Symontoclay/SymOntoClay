using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class AdditionOperatorHandler : BaseLoggedComponent, IBinaryOperatorHandler
    {
        public AdditionOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
        }

        private readonly IEngineContext _engineContext;

        public Value Call(Value leftOperand, Value rightOperand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            if(leftOperand.IsNumberValue && rightOperand.IsNumberValue)
            {
                return new NumberValue((double)(leftOperand.AsNumberValue.GetSystemValue()) + (double)(rightOperand.AsNumberValue.GetSystemValue()));
            }

            if(leftOperand.IsNullValue || rightOperand.IsNullValue)
            {
                return new NullValue();
            }

            throw new NotImplementedException();
        }
    }
}
