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
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Operator : CodeItem, IExecutable
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Operator;

        /// <inheritdoc/>
        public override bool IsOperator => true;

        /// <inheritdoc/>
        public override Operator AsOperator => this;

        public KindOfOperator KindOfOperator { get; set; } = KindOfOperator.Unknown;
        
        /// <inheritdoc/>
        public bool IsSystemDefined { get; set; }

        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        public ISystemHandler SystemHandler { get; set; }

        public List<FunctionArgumentInfo> Arguments { get; set; } = new List<FunctionArgumentInfo>();

        private Dictionary<StrongIdentifierValue, FunctionArgumentInfo> _argumentsDict;

        private IList<IFunctionArgument> _iArgumentsList;

        /// <inheritdoc/>
        public CodeItem CodeItem => this;

        /// <inheritdoc/>
        IList<IFunctionArgument> IExecutable.Arguments => _iArgumentsList;

        /// <inheritdoc/>
        public bool ContainsArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _argumentsDict.ContainsKey(name);
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
            var result = base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseOperatorWeight ^ (ulong)Math.Abs(KindOfOperator.GetHashCode());

            if (Arguments.IsNullOrEmpty())
            {
                _argumentsDict = new Dictionary<StrongIdentifierValue, FunctionArgumentInfo>();
                _iArgumentsList = new List<IFunctionArgument>();
            }
            else
            {
                _iArgumentsList = Arguments.Cast<IFunctionArgument>().ToList();

                _argumentsDict = Arguments.ToDictionary(p => p.Name, p => p);
            }

            return result;
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public Operator Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public Operator Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Operator)context[this];
            }

            var result = new Operator();
            context[this] = result;

            result.KindOfOperator = KindOfOperator;
            result.IsSystemDefined = IsSystemDefined;
            result.Statements = Statements.Select(p => p.CloneAstStatement(context)).ToList();
            result.CompiledFunctionBody = CompiledFunctionBody?.Clone(context);
            result.SystemHandler = SystemHandler;

            result.AppendCodeItem(this, context);

            return result;
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
                foreach(var item in Statements)
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
            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintObjListProp(n, nameof(Statements), Statements);
            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintShortObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintShortObjListProp(n, nameof(Statements), Statements);
            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsSystemDefined)} = {IsSystemDefined}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintBriefObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);
            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintExisting(n, nameof(SystemHandler), SystemHandler);

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
