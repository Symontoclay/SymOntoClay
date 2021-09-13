using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ActionInstanceValue: Value, IExecutable
    {
        public ActionInstanceValue(ActionPtr actionPtr, IStorage parentStorage)
        {
            _actionPtr = actionPtr;
            _parentStorage = parentStorage;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ActionInstanceValue;

        /// <inheritdoc/>
        public override bool IsActionInstanceValue => true;

        /// <inheritdoc/>
        public override ActionInstanceValue AsActionInstanceValue => this;

        public ActionInstance ActionInstance { get; private set; }

        private readonly ActionPtr _actionPtr;
        private readonly IStorage _parentStorage;

        /// <inheritdoc/>
        public IExecutionCoordinator TryActivate(IEngineContext context)
        {
            var actionInstance = new ActionInstance(_actionPtr, context, _parentStorage);

            actionInstance.Init();

            ActionInstance = actionInstance;

            return actionInstance.ExecutionCoordinator;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return ActionInstance;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            _actionPtr.CheckDirty();

            return base.CalculateLongHashCode() ^ _actionPtr.GetLongHashCode();
        }

        /// <inheritdoc/>
        public bool IsSystemDefined => ActionInstance.IsSystemDefined;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => ActionInstance.Arguments;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => ActionInstance.CompiledFunctionBody;

        /// <inheritdoc/>
        public CodeEntity CodeEntity => ActionInstance.CodeEntity;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => ActionInstance.SystemHandler;

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return ActionInstance.ContainsArgument(name);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new ActionInstanceValue(_actionPtr, _parentStorage);
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(_actionPtr), _actionPtr);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(_actionPtr), _actionPtr);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(_actionPtr), _actionPtr);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}ref: {_actionPtr.Name.NameValue}";
        }
    }
}
