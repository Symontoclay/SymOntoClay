using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class BinaryOperatorSystemHandler: ISystemHandler
    {
        public BinaryOperatorSystemHandler(IBinaryOperatorHandler operatorHandler, IEntityDictionary entityDictionary)
        {
            _leftOperandKey = entityDictionary.GetKey("leftOperand");
            _rightOperandKey = entityDictionary.GetKey("rightOperand");
            _operatorHandler = operatorHandler;
        }

        private ulong _leftOperandKey;
        private ulong _rightOperandKey;
        private IBinaryOperatorHandler _operatorHandler;

        /// <inheritdoc/>
        public Value Call(IList<Value> paramsList, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var leftOperand = paramsList[0];
            var rightOperand = paramsList[1];
            var anotation = paramsList[2];

            return _operatorHandler.Call(leftOperand, rightOperand, anotation, localCodeExecutionContext);
        }
        
        /// <inheritdoc/>
        public Value Call(IDictionary<ulong, Value> paramsDict, Value anotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var leftOperand = paramsDict[_leftOperandKey];
            var rightOperand = paramsDict[_rightOperandKey];

            return _operatorHandler.Call(leftOperand, rightOperand, anotation, localCodeExecutionContext);
        }
    }
}
