using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IFuzzyLogicStorage : ISpecificStorage
    {
        void Append(LinguisticVariable linguisticVariable);
        IList<WeightedInheritanceResultItem<FuzzyLogicNonNumericValue>> GetNonNumericValuesDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems);
        void AppendDefaultOperator(FuzzyLogicOperator fuzzyLogicOperator);
        FuzzyLogicOperator GetDefaultOperator(StrongIdentifierValue name);
    }
}
