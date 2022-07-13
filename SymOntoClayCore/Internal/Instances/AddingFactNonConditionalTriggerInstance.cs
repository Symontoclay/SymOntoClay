using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class AddingFactNonConditionalTriggerInstance: BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public AddingFactNonConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(context.Logger)
        {
            _executionCoordinator = parent.ExecutionCoordinator;
            _context = context;
            _parent = parent;
            _trigger = trigger;

#if DEBUG
            //Log($"_trigger = {_trigger}");
#endif

            var setBindingVariables = trigger.SetBindingVariables;

            if (setBindingVariables != null && setBindingVariables.Any())
            {
#if DEBUG
                //Log($"setBindingVariables = {setBindingVariables}");
#endif

                var varsList = setBindingVariables.GetTargetsList();

#if DEBUG
                //Log($"varsList = {varsList.WriteListToString()}");
#endif

                if(varsList.Any(p => p != StrongIdentifierValue.LogicalVarBlankIdentifier))
                {
                    var unxpectedVarsList = varsList.Where(p => p != StrongIdentifierValue.LogicalVarBlankIdentifier);

#if DEBUG
                    Log($"unxpectedVarsList = {unxpectedVarsList.WriteListToString()}");
#endif

                    if(unxpectedVarsList.Count() == 1)
                    {
                        throw new Exception($"The var `{unxpectedVarsList.Single().NameValue}` can not be bound in unconditional add fact trigger.");
                    }

                    throw new Exception($"The vars {string.Join(", ", unxpectedVarsList.Select(p => $"`{p.NameValue}`"))} can not be bound in unconditional add fact trigger.");
                }

                _factBindingVariable = setBindingVariables.GetDest(StrongIdentifierValue.LogicalVarBlankIdentifier);

#if DEBUG
                //Log($"_factBindingVariable = {_factBindingVariable}");
#endif

                _hasFactBindingVariable = true;
            }

            
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);

            _storage.LogicalStorage.OnAddingFact += LogicalStorage_OnAddingFact;
        }

        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IEngineContext _context;
        private readonly IStorage _storage;
        private readonly BaseInstance _parent;
        private readonly InlineTrigger _trigger;

        private readonly bool _hasFactBindingVariable;
        private readonly StrongIdentifierValue _factBindingVariable;

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
#if DEBUG
            //Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = _storage;

            localCodeExecutionContext.Holder = _parent.Name;

            var mutablePart = new MutablePartOfRuleInstance();
            mutablePart.Parent = ruleInstance;

            localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.AddingFact;
            localCodeExecutionContext.MutablePart = mutablePart;
            localCodeExecutionContext.AddedRuleInstance = ruleInstance;

#if DEBUG
            //Log($"localCodeExecutionContext = {localCodeExecutionContext}");
#endif

            if (_hasFactBindingVariable)
            {
                var storagesList = BaseResolver.GetStoragesList(localCodeExecutionContext.Storage);

                var targetStorage = storagesList.FirstOrDefault(p => p.Storage.Kind == KindOfStorage.Local);

                var targetVarStorage = targetStorage.Storage.VarStorage;

                targetVarStorage.SetValue(_factBindingVariable, new RuleInstanceValue(ruleInstance));
            }

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.SetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

#if DEBUG
            //Log($"processInitialInfo = {processInitialInfo}");
#endif

            var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo).AsTaskValue;

#if DEBUG
            //Log($"task = {task}");
#endif

            task.Wait();

            var result = new AddFactOrRuleResult() { KindOfResult = localCodeExecutionContext.KindOfAddFactResult };

            if(result.KindOfResult == KindOfAddFactOrRuleResult.Accept)
            {
#if DEBUG
                //Log($"mutablePart = {mutablePart}");
#endif

                if(mutablePart.ObligationModality != null || mutablePart.SelfObligationModality != null)
                {
                    result.MutablePart = mutablePart;
                }
            }

#if DEBUG
            Log($"result = {result}");
#endif

            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}
