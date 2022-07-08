using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.TriggersStorage
{
    public class EmptyTriggersStorage: BaseEmptySpecificStorage, ITriggersStorage
    {
        public EmptyTriggersStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(InlineTrigger inlineTrigger)
        {
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetSystemEventsTriggersDirectly(KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<InlineTrigger>>();
        }

        /// <inheritdoc/>
        public IList<WeightedInheritanceResultItem<InlineTrigger>> GetLogicConditionalTriggersDirectly(IList<WeightedInheritanceItem> weightedInheritanceItems)
        {
            return new List<WeightedInheritanceResultItem<InlineTrigger>>();
        }

        /// <inheritdoc/>
        public void Append(INamedTriggerInstance namedTriggerInstance)
        {
        }

        /// <inheritdoc/>
        public void Remove(INamedTriggerInstance namedTriggerInstance)
        {
        }

        /// <inheritdoc/>
        public event Action OnNamedTriggerInstanceChanged;

        /// <inheritdoc/>
        public event Action<IList<StrongIdentifierValue>> OnNamedTriggerInstanceChangedWithKeys;

        /// <inheritdoc/>
        public IList<INamedTriggerInstance> GetNamedTriggerInstancesDirectly(StrongIdentifierValue name)
        {
            return new List<INamedTriggerInstance>();
        }
    }
}
