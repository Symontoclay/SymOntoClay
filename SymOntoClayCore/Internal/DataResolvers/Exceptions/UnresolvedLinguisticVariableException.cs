using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.DataResolvers.Exceptions
{
    public class UnresolvedLinguisticVariableException: UnresolvedException
    {
        public UnresolvedLinguisticVariableException(StrongIdentifierValue name)
            : base($"Unresolved linguistic variable '{name.ForResolving.ToHumanizedLabel()}'.")
        {
        }
    }
}
