using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class PointOperatorHandler : BaseLoggedComponent, IBinaryOperatorHandler
    {
        public PointOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;
        }

        private readonly IEngineContext _engineContext;

        /// <inheritdoc/>
        public IndexedValue Call(IndexedValue leftOperand, IndexedValue rightOperand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"leftOperand = {leftOperand}");
            Log($"rightOperand = {rightOperand}");
            Log($"annotation = {annotation}");
#endif

            if(leftOperand.IsHostValue && rightOperand.IsStrongIdentifierValue)
            {
                return new PointRefValue(leftOperand.OriginalValue, rightOperand.OriginalValue).GetIndexed(_engineContext);
            }

            throw new NotImplementedException();
        }
    }
}
