using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
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
                Log($"setBindingVariables = {setBindingVariables}");
#endif

                var varsList = setBindingVariables.GetTargetsList();

#if DEBUG
                Log($"varsList = {varsList.WriteListToString()}");
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

                _hasFactBindingVariable = true;
            }

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;

            _localCodeExecutionContext.Holder = parent.Name;

            _storage.LogicalStorage.OnAddingFact += LogicalStorage_OnAddingFact;
        }

        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IEngineContext _context;
        private readonly IStorage _storage;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly BaseInstance _parent;
        private readonly InlineTrigger _trigger;

        private readonly bool _hasFactBindingVariable;

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
#if DEBUG
            Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");
#endif
            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.TrueValue }
            };
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
