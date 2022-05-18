using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IPackedInheritanceResolver
    {
        Value GetInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName);
        float GetRawInheritanceRank(StrongIdentifierValue subName, StrongIdentifierValue superName);
        IList<StrongIdentifierValue> GetSuperClassesKeysList(StrongIdentifierValue subName);
        IList<WeightedInheritanceItem> GetWeightedInheritanceItems(StrongIdentifierValue subName);
        bool IsFit(IList<StrongIdentifierValue> typeNamesList, Value value);
        uint? GetDistance(IList<StrongIdentifierValue> typeNamesList, Value value);
        uint? GetDistance(StrongIdentifierValue subName, StrongIdentifierValue superName);
    }
}
