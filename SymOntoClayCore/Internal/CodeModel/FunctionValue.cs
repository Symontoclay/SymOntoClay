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
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class FunctionValue : Value, IExecutable
    {
        public FunctionValue(Function function)
        {
            Function = function;
            _iExecutable = function;
        }

        public Function Function { get; private set; }
        private IExecutable _iExecutable;

        /// <inheritdoc/>
        IList<IFunctionArgument> IExecutable.Arguments => _iExecutable.Arguments;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => _iExecutable.CompiledFunctionBody;

        /// <inheritdoc/>
        public CodeItem CodeItem => _iExecutable.CodeItem;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.FunctionValue;

        /// <inheritdoc/>
        public override bool IsFunctionValue => true;

        /// <inheritdoc/>
        public bool IsSystemDefined => Function.IsSystemDefined;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => Function.SystemHandler;

        /// <inheritdoc/>
        public override FunctionValue AsFunctionValue => this;

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return Function.ContainsArgument(name);
        }

        /// <inheritdoc/>
        IExecutionCoordinator IExecutable.TryActivate(IEngineContext context)
        {
            return null;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [ResolveToType(typeof(LogicalValue))]
        public override IList<Value> WhereSection { get => Function.WhereSection; set => Function.WhereSection = value; }

        /// <inheritdoc/>
        public override IList<Annotation> Annotations { get => Function.Annotations; set => Function.Annotations = value; }

        /// <inheritdoc/>
        public override ulong GetLongConditionalHashCode(CheckDirtyOptions options)
        {
            return Function.GetLongConditionalHashCode(options);
        }

        /// <inheritdoc/>
        public override ulong GetLongHashCode(CheckDirtyOptions options)
        {
            return Function.GetLongHashCode(options);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongConditionalHashCode(CheckDirtyOptions options)
        {
            Function.CheckDirty(options);

            return 0u;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return Function.GetLongHashCode(options);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public FunctionValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public FunctionValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (FunctionValue)context[this];
            }

            var result = new FunctionValue(Function.CloneFunction(context));
            context[this] = result;

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            Function.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        public override Value GetAnnotationValue()
        {
            return Function.GetAnnotationValue();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Function), Function);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Function), Function);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Function), Function);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
