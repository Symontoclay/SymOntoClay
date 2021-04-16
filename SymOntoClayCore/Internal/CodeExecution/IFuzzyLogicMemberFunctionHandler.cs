using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IFuzzyLogicMemberFunctionHandler: IFuzzyLogicHandler
    {
        KindOfFuzzyLogicMemberFunction Kind { get; }
        NumberValue Defuzzificate();
        NumberValue Defuzzificate(IEnumerable<IFuzzyLogicOperatorHandler> operatorHandlers);
        void Check();
    }
}
