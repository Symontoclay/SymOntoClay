/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class InstanceInfo : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public InstanceInfo(StrongIdentifierValue name, IEngineContext context, IStorage parentStorage)
            : base(context.Logger)
        {
            Name = name;
            _context = context;
            IndexedName = Name.GetIndexed(context);

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;
            _localCodeExecutionContext.Holder = IndexedName;

#if DEBUG
            //Log($"_localCodeExecutionContext = {_localCodeExecutionContext}");
#endif

            _triggersResolver = new TriggersResolver(context);
        }

        public StrongIdentifierValue Name { get; private set; }
        public IndexedStrongIdentifierValue IndexedName { get; private set; }
        private readonly IEngineContext _context;
        private readonly IStorage _storage;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly TriggersResolver _triggersResolver;
        private InstanceState _instanceState = InstanceState.Created;
        private List<LogicConditionalTriggerInstanceInfo> _logicConditionalTriggersList = new List<LogicConditionalTriggerInstanceInfo>();

        public void Init()
        {
            _instanceState = InstanceState.Initializing;

            var targetSystemEventsTriggersList = _triggersResolver.ResolveSystemEventsTriggersList(KindOfSystemEventOfInlineTrigger.Init, IndexedName, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"targetSystemEventsTriggersList = {targetSystemEventsTriggersList.WriteListToString()}");
#endif

            if(targetSystemEventsTriggersList.Any())
            {
                targetSystemEventsTriggersList.Reverse();

                var processInitialInfoList = new List<ProcessInitialInfo>();

                foreach (var targetTrigger in targetSystemEventsTriggersList)
                {
                    var localCodeExecutionContext = new LocalCodeExecutionContext();

                    var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                    localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);

                    localCodeExecutionContext.Holder = IndexedName;

                    var processInitialInfo = new ProcessInitialInfo();
                    processInitialInfo.CompiledFunctionBody = targetTrigger.ResultItem.CompiledFunctionBody;
                    processInitialInfo.LocalContext = localCodeExecutionContext;
                    processInitialInfo.Metadata = targetTrigger.ResultItem.OriginalInlineTrigger.CodeEntity;

                    processInitialInfoList.Add(processInitialInfo);
                }

#if DEBUG
                //Log($"processInitialInfoList = {processInitialInfoList.WriteListToString()}");
#endif

                var taskValue = _context.CodeExecutor.ExecuteBatchAsync(processInitialInfoList);

#if DEBUG
                //Log($"taskValue = {taskValue}");
#endif
            }

            var targetLogicConditionalTriggersList = _triggersResolver.ResolveLogicConditionalTriggersList(IndexedName, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"targetLogicConditionalTriggersList = {targetLogicConditionalTriggersList.WriteListToString()}");
#endif

            if(targetLogicConditionalTriggersList.Any())
            {
                foreach(var targetTrigger in targetLogicConditionalTriggersList)
                {
#if DEBUG
                    //Log($"targetTrigger = {targetTrigger}");
#endif

                    var triggerInstanceInfo = new LogicConditionalTriggerInstanceInfo(targetTrigger.ResultItem, this, _context, _storage);
                    _logicConditionalTriggersList.Add(triggerInstanceInfo);
                }
            }

            _instanceState = InstanceState.Initialized;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            foreach(var triggerInstanceInfo in _logicConditionalTriggersList)
            {
                triggerInstanceInfo.Dispose();
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

            sb.PrintObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            //sb.AppendLine($"{spaces}{nameof(AddSelf)} = {AddSelf}");
            //sb.AppendLine($"{spaces}{nameof(JustDistinct)} = {JustDistinct}");

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

            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            //sb.AppendLine($"{spaces}{nameof(IsDeepMode)} = {IsDeepMode}");
            //sb.AppendLine($"{spaces}{nameof(AddSelf)} = {AddSelf}");
            //sb.AppendLine($"{spaces}{nameof(JustDistinct)} = {JustDistinct}");

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

            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            //sb.AppendLine($"{spaces}{nameof(IsDeepMode)} = {IsDeepMode}");
            //sb.AppendLine($"{spaces}{nameof(AddSelf)} = {AddSelf}");
            //sb.AppendLine($"{spaces}{nameof(JustDistinct)} = {JustDistinct}");

            return sb.ToString();
        }
    }
}
