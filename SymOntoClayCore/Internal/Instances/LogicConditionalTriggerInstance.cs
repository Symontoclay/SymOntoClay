/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers;
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
    public class LogicConditionalTriggerInstance : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public LogicConditionalTriggerInstance(InlineTrigger trigger, BaseInstance parent, IEngineContext context, IStorage parentStorage)
            : base(context.Logger)
        {
            _executionCoordinator = parent.ExecutionCoordinator;
            _context = context;
            _parent = parent;
            _trigger = trigger;

#if DEBUG
            //Log($"_trigger = {_trigger}");
            Log($"_trigger.SetCondition = {_trigger.SetCondition?.GetHumanizeDbgString()}");
            Log($"_trigger.ResetCondition = {_trigger.ResetCondition?.GetHumanizeDbgString()}");
#endif

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;

            _localCodeExecutionContext.Holder = parent.Name;

            _setConditionalTriggerObserver = new LogicConditionalTriggerObserver(context, _storage, _trigger.SetCondition);
            _setConditionalTriggerObserver.OnChanged += SetCondition_OnChanged;

            if (_trigger.ResetCondition != null)
            {
                _resetConditionalTriggerObserver = new LogicConditionalTriggerObserver(context, _storage, _trigger.ResetCondition);
                _resetConditionalTriggerObserver.OnChanged += ResetCondition_OnChanged;
            }

            //_storage.LogicalStorage.OnChanged += LogicalStorage_OnChanged;
        }

        private IExecutionCoordinator _executionCoordinator;
        private readonly IEngineContext _context;
        private BaseInstance _parent;
        private InlineTrigger _trigger;
        private readonly IStorage _storage;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly LogicConditionalTriggerObserver _setConditionalTriggerObserver;
        private readonly LogicConditionalTriggerObserver _resetConditionalTriggerObserver;

        public void Init()
        {
#if DEBUG
            Log("Begin");
#endif

            //throw new NotImplementedException();

#if DEBUG
            Log("End");
#endif
        }

        private void SetCondition_OnChanged()
        {
#if DEBUG
            Log("Begin");
#endif

#if DEBUG
            Log("End");
#endif
        }

        private void ResetCondition_OnChanged()
        {
#if DEBUG
            Log("Begin");
#endif

#if DEBUG
            Log("End");
#endif
        }

        //        /// <inheritdoc/>
        //        protected override BindingVariables GetBindingVariables()
        //        {
        //            return _trigger.BindingVariables;
        //        }

        //        /// <inheritdoc/>
        //        protected override void RunHandler(LocalCodeExecutionContext localCodeExecutionContext)
        //        {
        //#if DEBUG
        //            //Log($"_trigger = {_trigger}");
        //            //Log($"_trigger.CompiledFunctionBody = {_trigger.CompiledFunctionBody.ToDbgString()}");
        //#endif

        //            var processInitialInfo = new ProcessInitialInfo();
        //            processInitialInfo.CompiledFunctionBody = _trigger.CompiledFunctionBody;
        //            processInitialInfo.LocalContext = localCodeExecutionContext;
        //            processInitialInfo.Metadata = _trigger;
        //            processInitialInfo.Instance = _parent;
        //            processInitialInfo.ExecutionCoordinator = _executionCoordinator;

        //#if DEBUG
        //            //Log($"processInitialInfo = {processInitialInfo}");
        //#endif

        //            var task = _context.CodeExecutor.ExecuteAsync(processInitialInfo);
        //        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _setConditionalTriggerObserver.OnChanged -= SetCondition_OnChanged;
            _setConditionalTriggerObserver.Dispose();

            if(_resetConditionalTriggerObserver != null)
            {
                _resetConditionalTriggerObserver.OnChanged -= ResetCondition_OnChanged;
                _resetConditionalTriggerObserver.Dispose();
            }

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
