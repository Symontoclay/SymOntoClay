using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.StatesStorage
{
    public class EmptyStatesStorage : BaseEmptySpecificStorage, IStatesStorage
    {
        public EmptyStatesStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(StateDef state)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<StateDef>> GetStatesDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<StateDef>>();
        }

        /// <inheritdoc/>
        public List<StrongIdentifierValue> AllStateNames()
        {
            return new List<StrongIdentifierValue>();
        }

        /// <inheritdoc/>
        public List<StateDef> GetAllStatesListDirectly()
        {
            return new List<StateDef>();
        }

        /// <inheritdoc/>
        public void SetDefaultStateName(StrongIdentifierValue name)
        {
        }

        /// <inheritdoc/>
        public StrongIdentifierValue GetDefaultStateNameDirectly()
        {
            return null;
        }

        /// <inheritdoc/>
        public List<ActivationInfoOfStateDef> GetActivationInfoOfStateListDirectly()
        {
            return new List<ActivationInfoOfStateDef>();
        }

        /// <inheritdoc/>
        public void Append(MutuallyExclusiveStatesSet mutuallyExclusiveStatesSet)
        {
        }

        /// <inheritdoc/>
        public List<MutuallyExclusiveStatesSet> GetMutuallyExclusiveStatesSetsListDirectly()
        {
            return new List<MutuallyExclusiveStatesSet>();
        }
    }
}
