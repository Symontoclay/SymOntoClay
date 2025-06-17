using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class UnresolvedLinguisticVariableException: UnresolvedException
    {
        public UnresolvedLinguisticVariableException(StrongIdentifierValue name)
            : base($"Unresolved linguistic variable '{name.ForResolving.ToHumanizedLabel()}'.")
        {
        }
    }
}
