using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class VarTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public VarTriggerConditionNodeObserver(IEntityLogger logger, IStorage storage, TriggerConditionNode condition)
            : base(logger)
        {
            _varName = condition.Name;
            
#if DEBUG
            //Log($"_varName = {_varName}");
#endif

            storage.VarStorage.OnChangedWithKeys += VarStorage_OnChangedWithKeys;
            _storage = storage;
        }

        private readonly StrongIdentifierValue _varName;
        private readonly IStorage _storage;

        private void VarStorage_OnChangedWithKeys(StrongIdentifierValue varName)
        {
#if DEBUG
            //Log($"varName = {varName}");
            //Log($"_varName = {_varName}");
#endif

            if (_varName == varName)
            {
                EmitOnChanged();
            }            
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.VarStorage.OnChangedWithKeys -= VarStorage_OnChangedWithKeys;

            base.OnDisposed();
        }
    }
}
