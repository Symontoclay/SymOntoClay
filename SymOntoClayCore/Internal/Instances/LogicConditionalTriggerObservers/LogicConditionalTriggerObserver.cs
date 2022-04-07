using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class LogicConditionalTriggerObserver : BaseComponent
    {
        public LogicConditionalTriggerObserver(TriggerConditionNodeObserverContext context, TriggerConditionNode condition)
            : base(context.EngineContext.Logger)
        {
            _observersList = TriggerConditionNodeObserversCreator.CreateObservers(context, condition);

            foreach (var observer in _observersList)
            {
                observer.OnChanged += Observer_OnChanged;
            }
        }

        private List<BaseTriggerConditionNodeObserver> _observersList;

        public event Action OnChanged;

        private void Observer_OnChanged()
        {
            OnChanged?.Invoke();
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            if(!_observersList.IsNullOrEmpty())
            {
                foreach(var observer in _observersList)
                {
                    observer.OnChanged -= Observer_OnChanged;
                    observer.Dispose();
                }
            }

            base.OnDisposed();
        }
    }
}
