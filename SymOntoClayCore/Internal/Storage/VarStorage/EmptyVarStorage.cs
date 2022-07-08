using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.VarStorage
{
    public class EmptyVarStorage : BaseEmptySpecificStorage, IVarStorage
    {
        public EmptyVarStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void SetSystemValue(StrongIdentifierValue varName, Value value)
        {
        }

        /// <inheritdoc/>
        public Value GetSystemValueDirectly(StrongIdentifierValue varName)
        {
            return null;
        }

        /// <inheritdoc/>
        public void Append(Var varItem)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<Var>> GetVarDirectly(StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<Var>>();
        }

        /// <inheritdoc/>
        public Var GetLocalVarDirectly(StrongIdentifierValue name)
        {
            return null;
        }

        /// <inheritdoc/>
        public void SetValue(StrongIdentifierValue varName, Value value)
        {
        }

        /// <inheritdoc/>
        public event Action OnChanged;

        /// <inheritdoc/>
        public event Action<StrongIdentifierValue> OnChangedWithKeys;
    }
}
