using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.FuzzyLogic
{
    public class EmptyFuzzyLogicStorage : BaseEmptySpecificStorage, IFuzzyLogicStorage
    {
        public EmptyFuzzyLogicStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(LinguisticVariable linguisticVariable)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>> GetNonNumericValuesDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>>();
        }

        /// <inheritdoc/>
        public void AppendDefaultOperator(FuzzyLogicOperator fuzzyLogicOperator)
        {
        }

        /// <inheritdoc/>
        public FuzzyLogicOperator GetDefaultOperator(StrongIdentifierValue name)
        {
            return null;
        }
    }
}
