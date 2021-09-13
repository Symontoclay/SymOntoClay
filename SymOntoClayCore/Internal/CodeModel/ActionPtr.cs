using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ActionPtr: AnnotatedItem, IExecutable, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public ActionPtr(ActionDef action, Operator op)
        {
            _action = action;
            _operator = op;
            _iOp = op;
        }

        private readonly ActionDef _action;
        private readonly Operator _operator;
        private readonly IExecutable _iOp;

        public ActionDef Action => _action;
        public Operator Operator => _operator;

        public StrongIdentifierValue Name => _action.Name;

        /// <inheritdoc/>
        public bool IsSystemDefined => _iOp.IsSystemDefined;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => _iOp.Arguments;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => _iOp.CompiledFunctionBody;

        /// <inheritdoc/>
        public CodeEntity CodeEntity => _iOp.CodeEntity;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => _iOp.SystemHandler;

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return _iOp.ContainsArgument(name);
        }

        /// <inheritdoc/>
        IExecutionCoordinator IExecutable.TryActivate(IEngineContext context)
        {
            return null;
        }

        /// <inheritdoc/>
        public override IList<Value> WhereSection 
        { 
            get
            {
                return _action.WhereSection;
            }

            set
            {
                _action.WhereSection = value;
            }
        }

        /// <inheritdoc/>
        public override StrongIdentifierValue Holder 
        { 
            get
            {
                return _action.Holder;
            }

            set
            {
                _action.Holder = value;
            }
        }

        /// <inheritdoc/>
        public override IList<RuleInstance> Annotations 
        { 
            get
            {
                return _action.Annotations;
            }

            set
            {
                _action.Annotations = value;
            }
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public ActionPtr Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public ActionPtr Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (ActionPtr)context[this];
            }

            var result = new ActionPtr(_action.Clone(context), _operator.Clone(context));
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            _action?.DiscoverAllAnnotations(result);
            _operator?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            _action.CheckDirty();
            _operator.CheckDirty();

            var result = base.CalculateLongHashCode() ^ _action.GetLongHashCode() ^ _operator.GetLongHashCode();

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Action), Action);
            sb.PrintObjProp(n, nameof(Operator), Operator);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Action), Action);
            sb.PrintShortObjProp(n, nameof(Operator), Operator);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Action), Action);
            sb.PrintBriefObjProp(n, nameof(Operator), Operator);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
