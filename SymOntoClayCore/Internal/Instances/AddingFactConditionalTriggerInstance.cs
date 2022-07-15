using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.LogicalStorage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class AddingFactConditionalTriggerInstance : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public AddingFactConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(context.Logger)
        {
            _executionCoordinator = parent.ExecutionCoordinator;
            _context = context;
            _parent = parent;
            _trigger = trigger;

#if DEBUG
            //Log($"_trigger = {_trigger}");
#endif

            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);

            _storage.LogicalStorage.OnAddingFact += LogicalStorage_OnAddingFact;
        }

        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IEngineContext _context;
        private readonly IStorage _storage;
        private readonly BaseInstance _parent;
        private readonly InlineTrigger _trigger;

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
#if DEBUG
            Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");
            Log($"_trigger.SetCondition = {_trigger.SetCondition}");
#endif

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = _storage;

            localCodeExecutionContext.Holder = _parent.Name;

            var mutablePart = new MutablePartOfRuleInstance();
            mutablePart.Parent = ruleInstance;

            localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.AddingFact;
            localCodeExecutionContext.MutablePart = mutablePart;
            localCodeExecutionContext.AddedRuleInstance = ruleInstance;

            //var searchOptions = new LogicalSearchOptions();
            //searchOptions.QueryExpression = _condition;
            //searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

            return new AddFactOrRuleResult() { KindOfResult = KindOfAddFactOrRuleResult.Accept };
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.LogicalStorage.OnAddingFact -= LogicalStorage_OnAddingFact;

            base.OnDisposed();
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
