namespace SymOntoClay.Core.Internal.CodeExecution
{
    public enum KindOfCallResult
    {
        Unknown,
        Value,
        Values,
        NeedExecuteCode,
        NeedExecuteSetProperty,
        NeedExecuteGetProperty,
        WasSystemException,
        WasDslException,
        ExecutingCodeInOtherCodeFrame
    }
}
