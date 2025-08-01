namespace SymOntoClay.Core.Internal.CodeExecution
{
    public enum KindOfCallResult
    {
        Unknown,
        Value,
        NeedExecuteCode,
        NeedExecuteSetProperty,
        NeedExecuteGetProperty,
        WasSystemException,
        WasDslException
    }
}
