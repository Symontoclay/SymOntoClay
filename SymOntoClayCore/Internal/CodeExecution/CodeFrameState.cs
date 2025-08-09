namespace SymOntoClay.Core.Internal.CodeExecution
{
    public enum CodeFrameState
    {
        Init,
        BeginningCommandExecution,
        TakingCaller,
        ResolvingCallerInCodeFrame,
        ResolvedCaller,
        LoggingCall,
        TakingParameters,
        ResolvingParameterInCodeFrame,
        ResolvedParameters,
        CommandExecution,
        CommandPostExecution,
        EndCommandExecution
    }
}
