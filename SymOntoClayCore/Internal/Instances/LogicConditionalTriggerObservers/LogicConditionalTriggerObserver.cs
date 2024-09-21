/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class LogicConditionalTriggerObserver : BaseComponent, IOnChangedBaseTriggerConditionNodeObserverHandler
    {
        public LogicConditionalTriggerObserver(TriggerConditionNodeObserverContext context, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition, ILocalCodeExecutionContext localCodeExecutionContext)
            : base(context.EngineContext.Logger)
        {
            _observersList = TriggerConditionNodeObserversCreator.CreateObservers(context, condition, kindOfTriggerCondition, localCodeExecutionContext);

            foreach (var observer in _observersList)
            {
                observer.AddOnChangedHandler(this);
            }
        }

        private List<BaseTriggerConditionNodeObserver> _observersList;

        [Obsolete("Serialization Refactoring", true)] public event Action OnChanged;

        void IOnChangedBaseTriggerConditionNodeObserverHandler.Invoke()
        {
            Observer_OnChanged();
        }

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
                    observer.RemoveOnChangedHandler(this);
                    observer.Dispose();
                }
            }

            base.OnDisposed();
        }
    }
}
