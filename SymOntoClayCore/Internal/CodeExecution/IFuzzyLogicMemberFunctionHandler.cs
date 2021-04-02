using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IFuzzyLogicMemberFunctionHandler: IFuzzyLogicHandler
    {
        KindOfFuzzyLogicMemberFunction Kind { get; }
    }
}
