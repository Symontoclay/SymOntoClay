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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseAddingFactTriggerInstance : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        protected BaseAddingFactTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext, bool allowAddionalVariablesInBinding)
            : base(context.Logger)
        {
            _executionCoordinator = parent.ExecutionCoordinator;
            _context = context;
            _parentCodeExecutionContext = parentCodeExecutionContext;
            _parent = parent;
            _trigger = trigger;
            _baseResolver = context.DataResolversFactory.GetBaseResolver();

            var setBindingVariables = trigger.SetBindingVariables;

            if (setBindingVariables != null && setBindingVariables.Any())
            {
                var varsList = setBindingVariables.GetTargetsList();

                if(allowAddionalVariablesInBinding)
                {
                    if (varsList.Any(p => p == StrongIdentifierValue.LogicalVarBlankIdentifier))
                    {
                        _factBindingVariable = setBindingVariables.GetDest(StrongIdentifierValue.LogicalVarBlankIdentifier);

                        _hasFactBindingVariable = true;
                    }
                }
                else
                {
                    if (varsList.Any(p => p != StrongIdentifierValue.LogicalVarBlankIdentifier))
                    {
                        var unxpectedVarsList = varsList.Where(p => p != StrongIdentifierValue.LogicalVarBlankIdentifier);

#if DEBUG
                        Info("432176EA-72B9-4699-B784-7CF6C4041357", $"unxpectedVarsList = {unxpectedVarsList.WriteListToString()}");
#endif

                        if (unxpectedVarsList.Count() == 1)
                        {
                            throw new Exception($"The var `{unxpectedVarsList.Single().NameValue}` can not be bound in unconditional add fact trigger.");
                        }

                        throw new Exception($"The vars {string.Join(", ", unxpectedVarsList.Select(p => $"`{p.NameValue}`"))} can not be bound in unconditional add fact trigger.");
                    }

                    _factBindingVariable = setBindingVariables.GetDest(StrongIdentifierValue.LogicalVarBlankIdentifier);

                    _hasFactBindingVariable = true;
                }
            }

            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
        }

        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IEngineContext _context;
        protected readonly IStorage _storage;
        protected readonly ILocalCodeExecutionContext _parentCodeExecutionContext;
        private readonly BaseInstance _parent;
        private readonly InlineTrigger _trigger;
        private BaseResolver _baseResolver;

        private readonly bool _hasFactBindingVariable;
        private readonly StrongIdentifierValue _factBindingVariable;

        public void Init()
        {
            _storage.LogicalStorage.OnAddingFact += LogicalStorage_OnAddingFact;//no need
        }

        protected abstract IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance);

        protected IAddFactOrRuleResult ProcessAction(List<List<Var>> varsList, RuleInstance ruleInstance)
        {
            var localCodeExecutionContext = new LocalCodeExecutionContext(_parentCodeExecutionContext);
            localCodeExecutionContext.Storage = _storage;

            localCodeExecutionContext.Holder = _parent.Name;

            var mutablePart = new MutablePartOfRuleInstance();
            mutablePart.Parent = ruleInstance;

            localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.AddingFact;
            localCodeExecutionContext.MutablePart = mutablePart;
            localCodeExecutionContext.AddedRuleInstance = ruleInstance;

            var storagesList = _baseResolver.GetStoragesList(Logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var targetStorage = storagesList.FirstOrDefault(p => p.Storage.Kind == KindOfStorage.Local);

            var targetVarStorage = targetStorage.Storage.VarStorage;

            if (_hasFactBindingVariable)
            {
                ruleInstance.CheckDirty();

                targetVarStorage.SetValue(Logger, _factBindingVariable, ruleInstance);
            }

            if (varsList.Any())
            {
                var targetVarsList = varsList.First();

                foreach (var varItem in targetVarsList)
                {
                    targetVarStorage.Append(Logger, varItem);
                }
            }

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.SetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var task = _context.CodeExecutor.ExecuteAsync(Logger, processInitialInfo);

            task.Wait();

            var result = new AddFactOrRuleResult() { KindOfResult = localCodeExecutionContext.KindOfAddFactResult };

            if (result.KindOfResult == KindOfAddFactOrRuleResult.Accept)
            {
                if (mutablePart.ObligationModality != null || mutablePart.SelfObligationModality != null)
                {
                    result.MutablePart = mutablePart;
                }
            }

            return result;
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
