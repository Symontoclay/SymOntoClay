﻿using SymOntoClay.Core.Internal.CodeExecution;
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
    public abstract class Function: CodeItem, IExecutable
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Function;

        public List<FunctionArgumentInfo> Arguments { get; set; } = new List<FunctionArgumentInfo>();

        /// <inheritdoc/>
        IList<IFunctionArgument> IExecutable.Arguments => _iArgumentsList;

        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        public bool IsSystemDefined => false;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => null;

        private Dictionary<StrongIdentifierValue, FunctionArgumentInfo> _argumentsDict;

        private IList<IFunctionArgument> _iArgumentsList;

        /// <inheritdoc/>
        public CodeItem CodeItem => this;

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return _argumentsDict.ContainsKey(name);
        }

        public FunctionArgumentInfo GetArgument(StrongIdentifierValue name)
        {
            if (_argumentsDict.ContainsKey(name))
            {
                return _argumentsDict[name];
            }

            return null;
        }

        /// <inheritdoc/>
        IExecutionCoordinator IExecutable.TryActivate(IEngineContext context)
        {
            return null;
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            if (Arguments.IsNullOrEmpty())
            {
                _argumentsDict = new Dictionary<StrongIdentifierValue, FunctionArgumentInfo>();
                _iArgumentsList = new List<IFunctionArgument>();
            }
            else
            {
                _iArgumentsList = Arguments.Cast<IFunctionArgument>().ToList();

                _argumentsDict = Arguments.ToDictionary(p => p.Name, p => p);

                foreach (var argument in Arguments)
                {
                    result ^= argument.GetLongConditionalHashCode(options);
                }
            }

            return result;
        }

        public abstract Function CloneFunction(Dictionary<object, object> context);

        protected void AppendFuction(Function source, Dictionary<object, object> context)
        {
            Arguments = source.Arguments?.Select(p => p.Clone(context)).ToList();

            Statements = source.Statements.Select(p => p.CloneAstStatement(context)).ToList();

            CompiledFunctionBody = source.CompiledFunctionBody.Clone(context);

            AppendCodeItem(this, context);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!Arguments.IsNullOrEmpty())
            {
                foreach (var item in Arguments)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (!Statements.IsNullOrEmpty())
            {
                foreach (var item in Statements)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintObjListProp(n, nameof(Statements), Statements);
            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintShortObjListProp(n, nameof(Statements), Statements);
            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);
            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}