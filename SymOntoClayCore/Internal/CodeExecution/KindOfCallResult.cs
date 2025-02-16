namespace SymOntoClay.Core.Internal.CodeExecution
{
    public enum KindOfCallResult
    {
        Unknown,
        Value,
        NeenExecuteCode,
        NeedExecuteSetProperty,
        NeedExecuteGetProperty,
        WasSystemException,
        WasDslException
    }
}
