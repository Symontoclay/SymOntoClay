using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class UnaryPlusOperatorHandler : BaseLoggedComponent, IUnaryOperatorHandler
    {
        public UnaryPlusOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
        }

        /// <inheritdoc/>
        public Value Call(Value operand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"operand = {operand}");
            //Log($"annotation = {annotation}");
#endif

            if (operand.IsSystemNull)
            {
                return new NullValue();
            }

            if(operand.IsNumberValue)
            {
                var systemValue = operand.AsNumberValue.SystemValue.Value;

                if(systemValue < 0)
                {
                    systemValue *= -1;

                    return new NumberValue(systemValue);
                }

                return operand;
            }

            throw new NotImplementedException();
        }
    }
}
