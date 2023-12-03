/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
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

        /// <inheritdoc/>
        public override bool IsFunction => true;

        /// <inheritdoc/>
        public override Function AsFunction => this;

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
        public bool ContainsArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _argumentsDict.ContainsKey(name);
        }

        public FunctionArgumentInfo GetArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            if (_argumentsDict.ContainsKey(name))
            {
                return _argumentsDict[name];
            }

            return null;
        }

        /// <inheritdoc/>
        IExecutionCoordinator IExecutable.GetCoordinator(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return null;
        }

        /// <inheritdoc/>
        IExecutable IExecutable.Activate(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator)
        {
            return this;
        }

        /// <inheritdoc/>
        public ILocalCodeExecutionContext OwnLocalCodeExecutionContext => null;

        /// <inheritdoc/>
        public bool NeedActivation => false;

        /// <inheritdoc/>
        public bool IsActivated => false;

        /// <inheritdoc/>
        public UsingLocalCodeExecutionContextPreferences UsingLocalCodeExecutionContextPreferences => UsingLocalCodeExecutionContextPreferences.Default;

        /// <inheritdoc/>
        public bool IsInstance => false;

        /// <inheritdoc/>
        public IInstance AsInstance => null;

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
                    result ^= argument.GetLongHashCode(options);
                }
            }

            return result;
        }

        public abstract Function CloneFunction(Dictionary<object, object> context);

        protected void AppendFuction(Function source, Dictionary<object, object> context)
        {
            Arguments = source.Arguments?.Select(p => p.Clone(context)).ToList();

            Statements = source.Statements.Select(p => p.CloneAstStatement(context)).ToList();

            CompiledFunctionBody = source.CompiledFunctionBody?.Clone(context);

            AppendCodeItem(this, context);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
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

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}'{ToHumanizedString()}'";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder();

            if(options?.EnableMark ?? false)
            {
                sb.Append("fun");
            }

            if(Name != null && !IsAnonymous)
            {
                sb.Append($" {Name.NameValue}");
            }

            if(!Arguments.IsNullOrEmpty() || ((options?.EnableParamsIfEmpty ?? false) && Arguments.IsNullOrEmpty()))
            {
                sb.Append("(");

                if (!Arguments.IsNullOrEmpty())
                {
                    var argumentsList = new List<string>();

                    foreach (var argument in Arguments)
                    {
                        argumentsList.Add(argument.ToHumanizedString(options));
                    }

                    sb.Append(string.Join(", ", argumentsList));
                }

                sb.Append(")");
            }

            sb.AppendLine("{");

            if (!Statements.IsNullOrEmpty())
            {
                foreach(var statement in Statements)
                {
                    sb.AppendLine(statement.ToHumanizedString(options));
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            var result = new MonitoredHumanizedLabel()
            {
                KindOfCodeItemDescriptor = "fun"
            };

            result.Label = Name.NameValue;

            if (!Arguments.IsNullOrEmpty())
            {
                var signatures = new List<MonitoredHumanizedMethodArgument>();

                foreach (var argument in Arguments)
                {
                    signatures.Add(argument.ToMonitoredHumanizedMethodArgument(logger));
                }

                result.Signatures = signatures;
            }

            return result;
        }
    }
}
