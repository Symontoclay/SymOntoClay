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
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseAddingFactTriggerInstance : BaseComponent, IOnAddingFactHandler, IObjectToString, IObjectToShortString, IObjectToBriefString
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
                        var unexpectedVarsList = varsList.Where(p => p != StrongIdentifierValue.LogicalVarBlankIdentifier);

#if DEBUG
                        //Info("432176EA-72B9-4699-B784-7CF6C4041357", $"unexpectedVarsList = {unexpectedVarsList.WriteListToString()}");
#endif

                        if (unexpectedVarsList.Count() == 1)
                        {
                            throw new Exception($"The var `{unexpectedVarsList.Single().NameValue}` can not be bound in unconditional add fact trigger.");
                        }

                        throw new Exception($"The vars {string.Join(", ", unexpectedVarsList.Select(p => $"`{p.NameValue}`"))} can not be bound in unconditional add fact trigger.");
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
            _storage.LogicalStorage.AddOnAddingFactHandler(this);
        }

        IAddFactOrRuleResult IOnAddingFactHandler.OnAddingFact(IMonitorLogger logger, RuleInstance fact)
        {
            return LogicalStorage_OnAddingFact(fact);
        }

        protected abstract IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance);

        protected IAddFactOrRuleResult ProcessAction(List<List<VarInstance>> varsList, RuleInstance ruleInstance)
        {
            var localCodeExecutionContext = new LocalCodeExecutionContext(_parentCodeExecutionContext);
            localCodeExecutionContext.Storage = _storage;
            localCodeExecutionContext.Holder = _parent.Name;
            localCodeExecutionContext.Instance = _parent;

#if DEBUG
            //Info("B39B210C-1018-4E4C-BC76-857EFEECA25D", $"ruleInstance = {ruleInstance.ToHumanizedString()}");
#endif

            var ruleInstanceReference = new RuleInstanceReference(ruleInstance);

            localCodeExecutionContext.Kind = KindOfLocalCodeExecutionContext.AddingFact;
            localCodeExecutionContext.AddedRuleInstance = ruleInstanceReference;

            var storagesList = _baseResolver.GetStoragesList(Logger, localCodeExecutionContext.Storage, KindOfStoragesList.CodeItems);

            var targetStorage = storagesList.FirstOrDefault(p => p.Storage.Kind == KindOfStorage.Local);

            var targetVarStorage = targetStorage.Storage.VarStorage;

            AddOrReplaceTargetRuleInstanceInFactBindingVariable(varsList, ruleInstance, ruleInstanceReference, targetVarStorage, localCodeExecutionContext);

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.SetCompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger;
            processInitialInfo.Instance = _parent;
            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

            var task = _context.CodeExecutor.ExecuteAsync(Logger, processInitialInfo);

            task.Wait();

#if DEBUG
            //Info("08965690-19E4-4139-8C27-859B4243E22C", $"ruleInstanceReference = {ruleInstanceReference.ToHumanizedString()}");
            //Info("B6506D49-FD89-4468-9359-0630AC950104", $"localCodeExecutionContext.KindOfAddFactResult = {localCodeExecutionContext.KindOfAddFactResult}");
#endif

            var result = new AddFactOrRuleResult() { KindOfResult = localCodeExecutionContext.KindOfAddFactResult };

            if (result.KindOfResult == KindOfAddFactOrRuleResult.Accept)
            {
                result.ChangedRuleInstance = ruleInstanceReference.CurrentRuleInstance;
            }

            return result;
        }

        private void AddOrReplaceTargetRuleInstanceInFactBindingVariable(List<List<VarInstance>> varsList, RuleInstance ruleInstance, RuleInstanceReference ruleInstanceReference, IVarStorage targetVarStorage, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Info("EE50A5E4-0734-44C9-B8AB-AD80408A0043", $"_hasFactBindingVariable = {_hasFactBindingVariable}");
            //Info("3DDD3844-3B2F-4134-9791-99FE0CBA7F7B", $"_factBindingVariable = {_factBindingVariable}");
#endif

            VarInstance factBindingVariable = null;

            if (_hasFactBindingVariable)
            {
                ruleInstance.CheckDirty();

                factBindingVariable = GetFactBindingVariable(varsList);

#if DEBUG
                //Info("97C40515-8492-42A6-B785-9D29D74F8BCA", $"factBindingVariable = {factBindingVariable}");
#endif

                if(factBindingVariable == null)
                {
                    targetVarStorage.SetValue(Logger, _factBindingVariable, ruleInstanceReference, localCodeExecutionContext);
                }                
            }

            if (varsList.Any())
            {
                factBindingVariable ??= GetFactBindingVariable(varsList);

                var targetVarsList = varsList.First();

                foreach (var varItem in targetVarsList)
                {
#if DEBUG
                    //Info("5AACC87F-659C-4CF7-BCEA-728E46C9064C", $"varItem = {varItem}");
                    //Info("80ACB394-AC09-4D40-BE77-7EF5ED33585F", $"varItem.Value = {varItem.Value}");
#endif

                    if(varItem == factBindingVariable)
                    {
                        varItem.SetValue(Logger, ruleInstanceReference);
                    }

                    targetVarStorage.Append(Logger, varItem);
                }
            }
        }

        private VarInstance GetFactBindingVariable(List<List<VarInstance>> varsList)
        {
            if (varsList.Any())
            {
                var targetVarsList = varsList.First();

                return targetVarsList.FirstOrDefault(p => p.Name == _factBindingVariable);
            }

            return null;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.LogicalStorage.RemoveOnAddingFactHandler(this);

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
