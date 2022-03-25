using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IStatesStorage : ISpecificStorage
    {
        void Append(StateDef state);
        IList<WeightedInheritanceResultItem<StateDef>> GetStatesDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems);
        List<StrongIdentifierValue> AllStateNames();
        void SetDefaultStateName(StrongIdentifierValue name);
        StrongIdentifierValue GetDefaultStateNameDirectly();
        List<ActivationInfoOfStateDef> GetActivationInfoOfStateListDirectly();
        void Append(MutuallyExclusiveStatesSet mutuallyExclusiveStatesSet);
        List<MutuallyExclusiveStatesSet> GetMutuallyExclusiveStatesSetsListDirectly();
    }
}
