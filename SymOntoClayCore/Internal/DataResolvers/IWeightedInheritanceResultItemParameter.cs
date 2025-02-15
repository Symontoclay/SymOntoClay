using SymOntoClay.Common;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public interface IWeightedInheritanceResultItemParameter: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        bool HasConditionalSections { get; }
    }
}
