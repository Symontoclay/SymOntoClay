namespace SymOntoClay.Core.Internal.CodeExecution
{
    public enum CodeFrameState
    {
        Init,
        BeginningCommandExecution,
        TakingParameters,
        ResolvingParameters,
        ResolvingParameterInCodeFrame,
        ResolvedParameters,
        CommandExecution,
        CommandPostExecution,
        EndCommandExecution
    }
}
