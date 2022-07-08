using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.MethodsStorage
{
    public class EmptyMethodsStorage : BaseEmptySpecificStorage, IMethodsStorage
    {
        public EmptyMethodsStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(NamedFunction namedFunction)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<NamedFunction>> GetNamedFunctionsDirectly(StrongIdentifierValue name, int paramsCount, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<NamedFunction>>();
        }
    }
}
