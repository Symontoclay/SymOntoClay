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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerExecutors;
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class AddingFactConditionalTriggerInstance : BaseAddingFactTriggerInstance
    {
        public AddingFactConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(trigger, parent, context, parentStorage, parentCodeExecutionContext, true)
        {
            _triggerConditionNodeObserverContext = new TriggerConditionNodeObserverContext(context, _storage, parent.Name);

            _setConditionalTriggerExecutor = new LogicConditionalTriggerExecutor(_triggerConditionNodeObserverContext, trigger.SetCondition, KindOfTriggerCondition.SetCondition, trigger.SetBindingVariables, parentCodeExecutionContext);
        }

        private readonly TriggerConditionNodeObserverContext _triggerConditionNodeObserverContext;
        private readonly LogicConditionalTriggerExecutor _setConditionalTriggerExecutor;
        private List<RuleInstance> _processedItems = new List<RuleInstance>();
        private readonly object _checkProcessedItemsLockObj = new object();

        /// <inheritdoc/>
        protected override IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
            lock (_checkProcessedItemsLockObj)
            {
                if (_processedItems.Contains(ruleInstance))
                {
                    return null;
                }

                _processedItems.Add(ruleInstance);
            }

            var setResult = _setConditionalTriggerExecutor.Run(out List<List<Var>> varsList, ruleInstance);

            if(!setResult.IsSuccess)
            {
                return null;
            }

            return ProcessAction(varsList, ruleInstance);
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _processedItems.Clear();
            _processedItems = null;

            base.OnDisposed();
        }
    }
}
