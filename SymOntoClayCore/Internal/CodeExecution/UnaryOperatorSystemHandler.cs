using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class UnaryOperatorSystemHandler : ISystemHandler
    {
        public UnaryOperatorSystemHandler(IUnaryOperatorHandler operatorHandler, IEntityDictionary entityDictionary)
        {
            _operandKey = entityDictionary.GetKey("operand");
            _operatorHandler = operatorHandler;
        }

        private readonly ulong _operandKey;
        private readonly IUnaryOperatorHandler _operatorHandler;

        /// <inheritdoc/>
        public IndexedValue Call(IList<IndexedValue> paramsList, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var operand = paramsList[0];
            var anotation = paramsList[1];

            return _operatorHandler.Call(operand, anotation, localCodeExecutionContext);
        }

        /// <inheritdoc/>
        public IndexedValue Call(IDictionary<ulong, IndexedValue> paramsDict, IndexedValue anotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
            var operand = paramsDict[_operandKey];

            return _operatorHandler.Call(operand, anotation, localCodeExecutionContext);
        }
    }
}
