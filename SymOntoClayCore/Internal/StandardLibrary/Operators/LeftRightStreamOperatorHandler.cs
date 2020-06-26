using NLog.Fluent;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class LeftRightStreamOperatorHandler: BaseLoggedComponent, IBinaryOperatorHandler
    {
        public LeftRightStreamOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
        }

        private readonly IEngineContext _engineContext;

        public Value Call(Value leftOperand, Value rightOperand, IStorage localContext)
        {
#if DEBUG
            Log($"leftOperand = {leftOperand}");
            Log($"rightOperand = {rightOperand}");
#endif

            Value valueFromSource = null;

            if(leftOperand.KindOfValue == KindOfValue.IdentifierValue)
            {
                throw new NotImplementedException();
            }
            else
            {
                valueFromSource = leftOperand;
            }

#if DEBUG
            Log($"valueFromSource = {valueFromSource}");
#endif

            if(rightOperand.KindOfValue == KindOfValue.IdentifierValue)
            {
                var identifier = rightOperand.AsIdentifierValue;

#if DEBUG
                Log($"identifier = {identifier}");
#endif

                if(identifier.KindOfName == KindOfName.Channel)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new Exception("Right operand should be a channel.");
                }
            }
            else
            {
                throw new Exception("Right operand should be a channel.");
            }          
        }
    }
}
