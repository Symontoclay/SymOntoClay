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

namespace SymOntoClay.Core.Internal.Instances
{
    public class LogicConditionalTriggerInstanceInfo : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public LogicConditionalTriggerInstanceInfo(IndexedInlineTrigger trigger, InstanceInfo parent, IEngineContext context, IStorage parentStorage)
            : base(context.Logger)
        {
            _context = context;
            _parent = parent;
            _trigger = trigger;
            _condition = trigger.Condition;

            var dataResolversFactory = context.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;

            _localCodeExecutionContext.Holder = parent.IndexedName;

            _storage.LogicalStorage.OnChangedWithKeys += LogicalStorage_OnChangedWithKeys;
        }

        private readonly LogicalSearchResolver _searcher;
        private readonly object _lockObj = new object();
        private readonly IEngineContext _context;
        private readonly IStorage _storage;
        private IndexedInlineTrigger _trigger;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private InstanceInfo _parent;
        private IndexedRuleInstance _condition;
        private bool _isOn;
        private List<ulong> _usedKeysList = new List<ulong>();

        private void LogicalStorage_OnChangedWithKeys(IList<ulong> changedKeysList)
        {
            lock(_lockObj)
            {
#if DEBUG
                //Log($"changedKeysList = {JsonConvert.SerializeObject(changedKeysList)}");
#endif

                if (_usedKeysList.Any())
                {
                    if(_usedKeysList.Intersect(changedKeysList).Any())
                    {
                        DoSearch();
                    }
                    return;
                }

                DoSearch();
            }
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
            Log($"searchResult = {searchResult}");
            //Log($"result = {DebugHelperForLogicalSearchResult.ToString(searchResult, _context.Dictionary)}");
            //foreach(var usedKey in searchResult.UsedKeysList)
            //{
            //    Log($"usedKey = {usedKey}");
            //    Log($"_context.Dictionary.GetName(usedKey) = {_context.Dictionary.GetName(usedKey)}");
            //}
#endif

            _usedKeysList = searchResult.UsedKeysList;

#if DEBUG
            Log($"_isOn = {_isOn}");
#endif

            if (_isOn)
            {
                if (!searchResult.IsSuccess)
                {
                    _isOn = false;
                }
            }
            else
            {
                if(searchResult.IsSuccess)
                {
                    _isOn = true;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = _trigger.CompiledFunctionBody;
                    processInitialInfo.LocalContext = _localCodeExecutionContext;
                    processInitialInfo.Metadata = _trigger.OriginalInlineTrigger.CodeEntity;

                    var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo);
                }
            }

#if DEBUG
            Log($"_isOn (after) = {_isOn}");
#endif
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.LogicalStorage.OnChangedWithKeys -= LogicalStorage_OnChangedWithKeys;

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
