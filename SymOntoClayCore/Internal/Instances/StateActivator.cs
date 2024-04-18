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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class StateActivator : BaseSimpleConditionalTriggerInstance
    {
        public StateActivator(ActivationInfoOfStateDef activationInfoOfState, AppInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(activationInfoOfState.ActivatingConditions.Condition, parent, context, parentStorage, parentCodeExecutionContext)
        {
            _stateDef = activationInfoOfState.State;
            _stateName = _stateDef.Name;
            _appInstance = parent;
            _bindingVariables = activationInfoOfState.ActivatingConditions.BindingVariables;
            _baseResolver = context.DataResolversFactory.GetBaseResolver();
        }

        private StateDef _stateDef;
        private AppInstance _appInstance;
        private StrongIdentifierValue _stateName;
        private BindingVariables _bindingVariables;
        private BaseResolver _baseResolver;

        /// <inheritdoc/>
        protected override bool ShouldSearch()
        {
            return !_appInstance.IsStateActivated(Logger, _stateName);
        }

        /// <inheritdoc/>
        protected override BindingVariables GetBindingVariables()
        {
            return _bindingVariables;
        }

        /// <inheritdoc/>
        protected override void RunHandler(ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if (_bindingVariables.Any())
            {
                var storagesList = _baseResolver.GetStoragesList(Logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

                var targetStorage = storagesList.FirstOrDefault(p => p.Storage.Kind == KindOfStorage.Local);

                var targetVarStorage = targetStorage.Storage.VarStorage;

                var varNamesList = _bindingVariables.GetDestList();

                var varList = new List<Var>();

                foreach(var varName in varNamesList)
                {
                    var varItem = targetVarStorage.GetLocalVarDirectly(Logger, varName);

                    if(varItem == null)
                    {
                        throw new ArgumentNullException(nameof(varItem));
                    }

                    varList.Add(varItem);
                }

                _appInstance.ActivateState(Logger, _stateDef, varList);
            }
            else
            {
                _appInstance.ActivateState(Logger, _stateDef);
            }            
        }
    }
}
