/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class LogicConditionalTriggerInstanceInfo : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public LogicConditionalTriggerInstanceInfo(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(context.Logger)
        {
            _context = context;
            _parent = parent;

            _executionCoordinator = parent.ExecutionCoordinator;

            _trigger = trigger;
            _condition = trigger.Condition;

#if DEBUG
            //Log($"_condition = {DebugHelperForRuleInstance.ToString(_condition)}");
#endif

            var dataResolversFactory = context.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;

            _localCodeExecutionContext.Holder = parent.Name;

            _storage.LogicalStorage.OnChanged += LogicalStorage_OnChanged;

            lock (_lockObj)
            {
                DoSearch();
            }
        }

        private readonly LogicalSearchResolver _searcher;
        private readonly object _lockObj = new object();
        private readonly IEngineContext _context;
        private readonly IExecutionCoordinator _executionCoordinator;
        private readonly IStorage _storage;
        private InlineTrigger _trigger;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private BaseInstance _parent;
        private RuleInstance _condition;
        private bool _isOn;
        private List<string> _foundKeys = new List<string>();

        private bool _isBusy;
        private bool _needRepeat;

        private void LogicalStorage_OnChanged()
        {
            Task.Run(() => 
            {
                try
                {
                    lock (_lockObj)
                    {
                        if (_isBusy)
                        {
                            _needRepeat = true;
                            return;
                        }

                        _isBusy = true;
                        _needRepeat = false;
                    }

                    DoSearch();

                    while (true)
                    {
                        lock (_lockObj)
                        {
                            if (!_needRepeat)
                            {
                                _isBusy = false;
                                return;
                            }

                            _needRepeat = false;
                        }

                        DoSearch();
                    }
                }
                catch(Exception e)
                {
                    Error(e);
                }
            });
        }

        private void DoSearch()
        {
            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = _condition;
            searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;

#if DEBUG
            //Log($"searchOptions = {searchOptions}");
#endif

            var searchResult = _searcher.Run(searchOptions);

#if DEBUG
            //Log($"searchResult = {searchResult}");
            //Log($"_condition = {DebugHelperForRuleInstance.ToString(_condition)}");
            //Log($"searchResult = {DebugHelperForLogicalSearchResult.ToString(searchResult)}");
            //foreach(var usedKey in searchResult.UsedKeysList)
            //{
            //    Log($"usedKey = {usedKey}");
            //    Log($"_context.Dictionary.GetName(usedKey) = {_context.Dictionary.GetName(usedKey)}");
            //}
#endif

#if DEBUG
            //Log($"searchResult.Items.Count = {searchResult.Items.Count}");
#endif

            if (searchResult.IsSuccess)
            {
                if(searchResult.Items.Count == 0)
                {
                    ProcessResultWithNoItems();
                }
                else
                {
                    ProcessResultWithItems(searchResult);
                }                
            }
            else
            {
                CleansingPreviousResults();
            }
        }

        private void CleansingPreviousResults()
        {
#if DEBUG
            //Log("CleansingPreviousResults()");
#endif

            _isOn = false;
            _foundKeys.Clear();
        }

        private void ProcessResultWithNoItems()
        {
#if DEBUG
            //Log("ProcessResultWithNoItems()");
            //Log($"_isOn = {_isOn}");
#endif

            if (_isOn)
            {
                return;
            }

            _isOn = true;

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            var processInitialInfo = new ProcessInitialInfo();
            processInitialInfo.CompiledFunctionBody = _trigger.CompiledFunctionBody;
            processInitialInfo.LocalContext = localCodeExecutionContext;
            processInitialInfo.Metadata = _trigger.CodeEntity;

            var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo, _executionCoordinator);
        }

        private void ProcessResultWithItems(LogicalSearchResult searchResult)
        {
#if DEBUG
            //Log("ProcessResultWithItems()");
#endif

            var usedKeys = new List<string>();
            var keysForAdding = new List<string>();

            var bindingVariables = _trigger.BindingVariables;
            
            foreach (var foundResultItem in searchResult.Items)
            {
#if DEBUG
                //Log($"foundResultItem = {foundResultItem}");
                //Log($"foundResultItem.KeyForTrigger = {foundResultItem.KeyForTrigger}");
#endif

                var keyForTrigger = foundResultItem.KeyForTrigger;

                usedKeys.Add(keyForTrigger);

                if (_foundKeys.Contains(keyForTrigger))
                {
                    continue;
                }

                keysForAdding.Add(keyForTrigger);
                _foundKeys.Add(keyForTrigger);

#if DEBUG
                //Log("Next");
#endif

                var localCodeExecutionContext = new LocalCodeExecutionContext();
                var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                var storage = new LocalStorage(localStorageSettings);
                localCodeExecutionContext.Storage = storage;
                localCodeExecutionContext.Holder = _parent.Name;

                if (bindingVariables.Any())
                {
                    var varStorage = storage.VarStorage;

                    var resultVarsList = foundResultItem.ResultOfVarOfQueryToRelationList;

#if DEBUG
                    //Log($"resultVarsList.Count = {resultVarsList.Count}");
#endif

                    if (bindingVariables.Count != resultVarsList.Count)
                    {
                        throw new NotImplementedException();
                    }

                    foreach (var resultVar in resultVarsList)
                    {
#if DEBUG
                        //Log($"resultVar = {resultVar}");
#endif

                        Value value = null;

                        var foundExpression = resultVar.FoundExpression;

                        var kindOfFoundExpression = foundExpression.Kind;

                        switch (kindOfFoundExpression)
                        {
                            case KindOfLogicalQueryNode.Entity:
                            case KindOfLogicalQueryNode.Concept:
                                value = foundExpression.Name;
                                break;

                            case KindOfLogicalQueryNode.Value:
                                value = foundExpression.Value;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfFoundExpression), kindOfFoundExpression, null);
                        }

#if DEBUG
                        //Log($"value = {value}");
#endif

                        var destVar = bindingVariables.GetDest(resultVar.NameOfVar);

#if DEBUG
                        //Log($"destVar = {destVar}");
#endif

                        varStorage.SetValue(destVar, value);
                    }
                }

                var processInitialInfo = new ProcessInitialInfo();
                processInitialInfo.CompiledFunctionBody = _trigger.CompiledFunctionBody;
                processInitialInfo.LocalContext = localCodeExecutionContext;
                processInitialInfo.Metadata = _trigger.CodeEntity;

                var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo, _executionCoordinator);
            }

            //var keysForRemoving = _foundKeys.Except(usedKeys);

            _foundKeys = usedKeys;

//#if DEBUG
//            Log($"_foundKeys = {JsonConvert.SerializeObject(_foundKeys)}");
//            Log($"usedKeys = {JsonConvert.SerializeObject(usedKeys)}");
//            Log($"keysForAdding = {JsonConvert.SerializeObject(keysForAdding)}");
//            Log($"keysForRemoving = {JsonConvert.SerializeObject(keysForRemoving)}");
//#endif
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.LogicalStorage.OnChanged -= LogicalStorage_OnChanged;

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
