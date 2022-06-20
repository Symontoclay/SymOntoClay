/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
        public override string ToSystemString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _actionPtr.CheckDirty();

            return base.CalculateLongHashCode(options) ^ _actionPtr.GetLongHashCode();
        }

        /// <inheritdoc/>
        public bool IsSystemDefined => ActionInstance.IsSystemDefined;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => ActionInstance.Arguments;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => ActionInstance.CompiledFunctionBody;

        /// <inheritdoc/>
        public CodeItem CodeItem => ActionInstance.CodeItem;

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
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return $"ref: {_actionPtr.Name.NameValue}";
        }
    }
}
