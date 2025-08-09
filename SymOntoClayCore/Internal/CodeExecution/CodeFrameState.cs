namespace SymOntoClay.Core.Internal.CodeExecution
{
    public enum CodeFrameState
    {
        Init,
        BeginningCommandExecution,
        TakingCaller,
        ResolvingCallerInCodeFrame,
        ResolvedCaller,
        TakingParameters,
        ResolvingParameterInCodeFrame,
        ResolvedParameters,
        CommandExecution,
        CommandPostExecution,
        EndCommandExecution
    }
}
