using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class InstanceInfo : BaseLoggedComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public InstanceInfo(StrongIdentifierValue name, IEngineContext context, IStorage parentStorage)
            : base(context.Logger)
        {
            _name = name;
            _context = context;
            _indexedName = _name.GetIndexed(context);

            _localCodeExecutionContext = new LocalCodeExecutionContext();
            var localStorageSettings = RealStorageSettingsHelper.Create(context, parentStorage);
            _storage = new LocalStorage(localStorageSettings);
            _localCodeExecutionContext.Storage = _storage;
            _localCodeExecutionContext.Holder = _indexedName;

#if DEBUG
            //Log($"_localCodeExecutionContext = {_localCodeExecutionContext}");
#endif

            _triggersResolver = new TriggersResolver(context);
        }

        private readonly StrongIdentifierValue _name;
        private readonly IndexedStrongIdentifierValue _indexedName;
        private readonly IEngineContext _context;
        private readonly IStorage _storage;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;
        private readonly TriggersResolver _triggersResolver;
        private InstanceState _instanceState = InstanceState.Created;

        /// <inheritdoc/>
        public void Init()
        {
            _instanceState = InstanceState.Initializing;

            var targetTriggersList = _triggersResolver.ResolveSystemEventsTriggersList(KindOfSystemEventOfInlineTrigger.Init, _indexedName, _localCodeExecutionContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"targetTriggersList = {targetTriggersList.WriteListToString()}");
#endif

            var codeFramesList = new List<CodeFrame>();

            foreach(var targetTrigger in targetTriggersList)
            {
                var localCodeExecutionContext = new LocalCodeExecutionContext();

                var localStorageSettings = RealStorageSettingsHelper.Create(_context, _storage);
                localCodeExecutionContext.Storage = new LocalStorage(localStorageSettings);

                localCodeExecutionContext.Holder = _indexedName;

                var codeFrame = new CodeFrame();
                codeFrame.CompiledFunctionBody = targetTrigger.ResultItem.CompiledFunctionBody;
                codeFrame.LocalContext = localCodeExecutionContext;

                codeFramesList.Add(codeFrame);
            }

#if DEBUG
            //Log($"codeFramesList = {codeFramesList.WriteListToString()}");
#endif

            var taskValue = _context.CodeExecutor.ExecuteBatchAsync(codeFramesList);

#if DEBUG
            //Log($"taskValue = {taskValue}");
#endif

            _instanceState = InstanceState.Initialized;
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

            sb.PrintObjProp(n, nameof(_name), _name);
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

            sb.PrintShortObjProp(n, nameof(_name), _name);
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

            sb.PrintBriefObjProp(n, nameof(_name), _name);
            sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            //sb.AppendLine($"{spaces}{nameof(IsDeepMode)} = {IsDeepMode}");
            //sb.AppendLine($"{spaces}{nameof(AddSelf)} = {AddSelf}");
            //sb.AppendLine($"{spaces}{nameof(JustDistinct)} = {JustDistinct}");

            return sb.ToString();
        }
    }
}
