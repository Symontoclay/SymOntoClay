using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class UnaryOperatorTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public UnaryOperatorTriggerConditionNodeObserver(IEntityLogger logger, IStorage storage, TriggerConditionNode condition)
            : base(logger)
        {
            _kindOfOperator = condition.KindOfOperator;

#if DEBUG
            //Log($"_kindOfOperator = {_kindOfOperator}");
#endif

            if (_kindOfOperator == KindOfOperator.CallFunction)
            {
                storage.VarStorage.OnChangedWithKeys += VarStorage_OnChangedWithKeys;
            }

            _storage = storage;
        }

        private readonly KindOfOperator _kindOfOperator;
        private readonly IStorage _storage;

        private void VarStorage_OnChangedWithKeys(StrongIdentifierValue varName)
        {
#if DEBUG
            //Log($"varName = {varName}");
            //Log($"_varName = {_varName}");
#endif

            EmitOnChanged();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            if(_kindOfOperator == KindOfOperator.CallFunction)
            {
                _storage.VarStorage.OnChangedWithKeys -= VarStorage_OnChangedWithKeys;
            }

            base.OnDisposed();
        }
    }
}
