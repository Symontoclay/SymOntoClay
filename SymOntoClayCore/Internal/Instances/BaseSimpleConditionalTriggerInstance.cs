/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public abstract class BaseSimpleConditionalTriggerInstance : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        protected BaseSimpleConditionalTriggerInstance(RuleInstance condition, BaseInstance parent, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(context.Logger)
        {
            _context = context;
            _parent = parent;


            _condition = condition;

            var dataResolversFactory = context.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext(parentCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = _storage;

            localCodeExecutionContext.Holder = parent.Name;

            _localCodeExecutionContext = localCodeExecutionContext;

            _storage.LogicalStorage.OnChanged += LogicalStorage_OnChanged;

            _searchOptions = new LogicalSearchOptions();
            _searchOptions.QueryExpression = _condition;
            _searchOptions.LocalCodeExecutionContext = _localCodeExecutionContext;

        }

        private readonly LogicalSearchResolver _searcher;
        private readonly object _lockObj = new object();
        protected readonly IEngineContext _context;
        private readonly IStorage _storage;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;
        protected BaseInstance _parent;
        private RuleInstance _condition;
        private LogicalSearchOptions _searchOptions;
        private bool _isOn;
        private List<string> _foundKeys = new List<string>();

        private bool _isBusy;
        private bool _needRepeat;

        public void Init()
        {
            lock (_lockObj)
            {
                LogicalStorage_OnChanged();
            }
        }

        protected abstract void RunHandler(ILocalCodeExecutionContext localCodeExecutionContext);

        protected virtual BindingVariables GetBindingVariables()
        {
            throw new NotImplementedException();
        }

        protected virtual bool ShouldSearch()
        {
            return true;
        }

        private void LogicalStorage_OnChanged()
        {
            Task.Run(() =>
            {
                try
                {
                    if(!ShouldSearch())
                    {
                        return;
                    }

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
                catch (Exception e)
                {
                    Error("9A9F4635-7A86-4074-9FD1-44E20C6D37FC", e);
                }
            });
        }

        private void DoSearch()
        {
            var searchResult = _searcher.Run(_searchOptions);

            if (searchResult.IsSuccess)
            {
                if (searchResult.Items.Count == 0)
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
            _isOn = false;
            _foundKeys.Clear();
        }

        private void ProcessResultWithNoItems()
        {
            if (_isOn)
            {
                return;
            }

            _isOn = true;

            var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
            var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
            var storage = new LocalStorage(localStorageSettings);
            localCodeExecutionContext.Storage = storage;
            localCodeExecutionContext.Holder = _parent.Name;

            RunHandler(localCodeExecutionContext);
        }

        private void ProcessResultWithItems(LogicalSearchResult searchResult)
        {
            var usedKeys = new List<string>();

            var bindingVariables = GetBindingVariables();

            foreach (var foundResultItem in searchResult.Items)
            {
                var keyForTrigger = foundResultItem.KeyForTrigger;

                usedKeys.Add(keyForTrigger);

                if (_foundKeys.Contains(keyForTrigger))
                {
                    continue;
                }


                var localCodeExecutionContext = new LocalCodeExecutionContext(_localCodeExecutionContext);
                var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                var storage = new LocalStorage(localStorageSettings);
                localCodeExecutionContext.Storage = storage;
                localCodeExecutionContext.Holder = _parent.Name;

                if (bindingVariables.Any())
                {
                    var varStorage = storage.VarStorage;

                    var resultVarsList = foundResultItem.ResultOfVarOfQueryToRelationList;

                    if (bindingVariables.Count != resultVarsList.Count)
                    {
                        throw new NotImplementedException();
                    }

                    foreach (var resultVar in resultVarsList)
                    {
                        var value = LogicalQueryNodeHelper.ToValue(resultVar.FoundExpression);

                        var destVar = bindingVariables.GetDest(resultVar.NameOfVar);

                        varStorage.SetValue(destVar, value);
                    }
                }

                RunHandler(localCodeExecutionContext);
            }


            _foundKeys = usedKeys;

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
