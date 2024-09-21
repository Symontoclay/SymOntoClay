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

using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public abstract class BaseTriggerConditionNodeObserver : BaseComponent
    {
        protected BaseTriggerConditionNodeObserver(IMonitorLogger logger)
            : base(logger)
        {
        }

        public void AddOnChangedHandler(IOnChangedBaseTriggerConditionNodeObserverHandler handler)
        {
            lock(_onChangedHandlersLockObj)
            {
                if(_onChangedHandlers.Contains(handler))
                {
                    return;
                }

                _onChangedHandlers.Add(handler);
            }
        }

        public void RemoveOnChangedHandler(IOnChangedBaseTriggerConditionNodeObserverHandler handler)
        {
            lock (_onChangedHandlersLockObj)
            {
                if (_onChangedHandlers.Contains(handler))
                {
                    _onChangedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnChangedHandlers()
        {
            lock (_onChangedHandlersLockObj)
            {
                foreach (var handler in _onChangedHandlers)
                {
                    handler.Invoke();
                }
            }
        }

        private object _onChangedHandlersLockObj = new object();
        private List<IOnChangedBaseTriggerConditionNodeObserverHandler> _onChangedHandlers = new List<IOnChangedBaseTriggerConditionNodeObserverHandler>();

        protected void EmitOnChanged()
        {
            EmitOnChangedHandlers();
        }
    }
}
