using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class VarTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public VarTriggerConditionNodeObserver(IEntityLogger logger, IStorage storage)
            : base(logger)
        {
            storage.VarStorage.OnChangedWithKeys += VarStorage_OnChangedWithKeys;
            _storage = storage;
        }

        private readonly IStorage _storage;

        private void VarStorage_OnChangedWithKeys(CodeModel.StrongIdentifierValue varName)
        {
#if DEBUG
            Log($"varName = {varName}");
#endif

            EmitOnChanged();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.VarStorage.OnChangedWithKeys -= VarStorage_OnChangedWithKeys;

            base.OnDisposed();
        }
    }
}
