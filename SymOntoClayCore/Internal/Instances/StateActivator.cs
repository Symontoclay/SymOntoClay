﻿using SymOntoClay.Core.Internal.CodeExecution;
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
    public class StateActivator : BaseTriggerInstance
    {
        public StateActivator(ActivationInfoOfStateDef activationInfoOfState, AppInstance parent, IEngineContext context, IStorage parentStorage)
            : base(activationInfoOfState.ActivatingConditions.Condition, parent, context, parentStorage)
        {
            _stateDef = activationInfoOfState.State;
            _stateName = _stateDef.Name;
            _appInstance = parent;
            _bindingVariables = activationInfoOfState.ActivatingConditions.BindingVariables;
        }

        private StateDef _stateDef;
        private AppInstance _appInstance;
        private StrongIdentifierValue _stateName;
        private BindingVariables _bindingVariables;

        /// <inheritdoc/>
        protected override bool ShouldSearch()
        {
#if DEBUG
            //Log($"_stateName = {_stateName}");
#endif

            return !_appInstance.IsStateActivated(_stateName);
        }

        /// <inheritdoc/>
        protected override BindingVariables GetBindingVariables()
        {
            return _bindingVariables;
        }

        /// <inheritdoc/>
        protected override void RunHandler(LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log("Begin");
            //Log($"_bindingVariables.Any() = {_bindingVariables.Any()}");
#endif

            if (_bindingVariables.Any())
            {
                var storagesList = BaseResolver.GetStoragesList(localCodeExecutionContext.Storage);

                var targetStorage = storagesList.FirstOrDefault(p => p.Storage.Kind == KindOfStorage.Local);

                var targetVarStorage = targetStorage.Storage.VarStorage;

                var varNamesList = _bindingVariables.GetDestList();

#if DEBUG
                //Log($"varNamesList = {varNamesList.WriteListToString()}");
#endif

                var varList = new List<Var>();

                foreach(var varName in varNamesList)
                {

#if DEBUG
                    //Log($"varName = {varName}");
#endif

                    var varItem = targetVarStorage.GetLocalVarDirectly(varName);

#if DEBUG
                    //Log($"varItem = {varItem}");
#endif

                    if(varItem == null)
                    {
                        throw new ArgumentNullException(nameof(varItem));
                    }

                    varList.Add(varItem);
                }

                _appInstance.ActivateState(_stateDef, varList);
            }
            else
            {
                _appInstance.ActivateState(_stateDef);
            }            
        }
    }
}
