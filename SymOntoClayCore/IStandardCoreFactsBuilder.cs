using SymOntoClay.Core.Internal.CodeModel;

namespace SymOntoClay.Core
{
    public interface IStandardCoreFactsBuilder
    {
        LogicalQueryNode BuildPropertyVirtualRelationInstance(StrongIdentifierValue propertyName, StrongIdentifierValue propertyInstanceName, Value propertyValue);
        RuleInstance BuildImplicitPropertyQueryInstance(StrongIdentifierValue propertyName, StrongIdentifierValue propertyInstanceName);
    }
}
