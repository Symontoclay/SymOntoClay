using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Handlers
{
    public abstract class BaseSpecialHandler : IExecutable, ISystemHandler
    {
        /// <inheritdoc/>
        public abstract Value Call(IList<Value> paramsList, LocalCodeExecutionContext localCodeExecutionContext);

        /// <inheritdoc/>
        public abstract Value Call(IDictionary<string, Value> paramsDict, Value anotation, LocalCodeExecutionContext localCodeExecutionContext);

        /// <inheritdoc/>
        public bool IsSystemDefined => true;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => null;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => null;

        /// <inheritdoc/>
        public CodeItem CodeItem => throw new NotImplementedException();

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => this;

        /// <inheritdoc/>
        public IExecutionCoordinator TryActivate(IEngineContext context)
        {
            return null;
        }

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return false;
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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");

            sb.PrintObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

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
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");

            sb.PrintShortObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

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
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");

            sb.PrintBriefObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

            return sb.ToString();
        }
    }
}
