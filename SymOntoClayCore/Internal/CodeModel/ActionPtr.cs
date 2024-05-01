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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
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
        public CodeItem CodeItem => _iOp.CodeItem;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => _iOp.SystemHandler;

        /// <inheritdoc/>
        public bool ContainsArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _iOp.ContainsArgument(logger, name);
        }

        /// <inheritdoc/>
        IExecutionCoordinator IExecutable.GetCoordinator(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return null;
        }

        /// <inheritdoc/>
        IExecutable IExecutable.Activate(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator)
        {
            return context.InstancesStorage.CreateInstance(logger, this, localCodeExecutionContext, executionCoordinator).AsActionInstanceValue.ActionInstance;
        }

        /// <inheritdoc/>
        public ILocalCodeExecutionContext OwnLocalCodeExecutionContext => null;

        /// <inheritdoc/>
        public StrongIdentifierValue Holder => _action.Holder;

        /// <inheritdoc/>
        public bool NeedActivation => true;

        /// <inheritdoc/>
        public bool IsActivated => false;

        /// <inheritdoc/>
        public UsingLocalCodeExecutionContextPreferences UsingLocalCodeExecutionContextPreferences => UsingLocalCodeExecutionContextPreferences.UseBothOwnAndCallerAsParent;

        /// <inheritdoc/>
        public bool IsInstance => false;

        /// <inheritdoc/>
        public IInstance AsInstance => null;

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
        public override IList<Annotation> Annotations 
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
        public override void AddAnnotation(Annotation annotation)
        {
            _action.AddAnnotation(annotation);
        }

        /// <inheritdoc/>
        public override IList<RuleInstance> AnnotationFacts => _action.AnnotationFacts;

        /// <inheritdoc/>
        public override IList<Value> MeaningRolesList => _action.MeaningRolesList;

        /// <inheritdoc/>
        public override Value GetSettings(StrongIdentifierValue key)
        {
            return _action.GetSettings(key);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public ActionPtr Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
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
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            _action?.DiscoverAllAnnotations(result);
            _operator?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _action.CheckDirty();
            _operator.CheckDirty();

            var result = base.CalculateLongHashCode(options) ^ _action.GetLongHashCode(options) ^ _operator.GetLongHashCode(options);

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

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
