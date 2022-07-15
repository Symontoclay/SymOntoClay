using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public static class ValueResolvingHelper
    {
        public static Value TryResolveFromVar(Value operand, LocalCodeExecutionContext localCodeExecutionContext, VarsResolver varsResolver)
        {
            if (operand.IsStrongIdentifierValue)
            {
                var identifier = operand.AsStrongIdentifierValue;

                if (identifier.KindOfName == KindOfName.Var || identifier.KindOfName == KindOfName.SystemVar)
                {
                    return varsResolver.GetVarValue(identifier, localCodeExecutionContext);
                }
            }

            return operand;
        }
    }
}
