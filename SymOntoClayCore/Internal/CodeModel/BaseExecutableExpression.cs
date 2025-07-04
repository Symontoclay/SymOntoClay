using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseExecutableExpression : CodeItem, IExecutable, IReturnable
    {
        protected BaseExecutableExpression()
        {
        }

        protected BaseExecutableExpression(AstExpression expression, CompiledFunctionBody compiledFunctionBody)
        {
            Expression = expression;
            CompiledFunctionBody = compiledFunctionBody;
        }

        protected BaseExecutableExpression(AstExpression expression, List<StrongIdentifierValue> typesList, CompiledFunctionBody compiledFunctionBody)
        {
            Expression = expression;
            CompiledFunctionBody = compiledFunctionBody;
            TypesList = typesList;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.ExecutableExpression;

        /// <inheritdoc/>
        public override bool IsBaseExecutableExpression => true;

        /// <inheritdoc/>
        public override BaseExecutableExpression AsBaseExecutableExpression => this;

        /// <inheritdoc/>
        public override bool IsIReturnable => true;

        /// <inheritdoc/>
        public override IReturnable AsIReturnable => this;

        /// <inheritdoc/>
        IList<IFunctionArgument> IExecutable.Arguments => _iArgumentsList;
        private IList<IFunctionArgument> _iArgumentsList = new List<IFunctionArgument>();

        public List<StrongIdentifierValue> TypesList { get; private set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        IList<StrongIdentifierValue> IReturnable.TypesList => TypesList;

        public AstExpression Expression { get; private set; }

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody { get; private set; }

        /// <inheritdoc/>
        public bool IsSystemDefined => false;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => null;

        /// <inheritdoc/>
        public CodeItem CodeItem => this;

        /// <inheritdoc/>
        public bool ContainsArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return false;
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

        public abstract BaseExecutableExpression CloneExecutableExpression(Dictionary<object, object> context);

        protected void AppendExecutableExpression(BaseExecutableExpression source, Dictionary<object, object> context)
        {
            TypesList = source.TypesList?.Select(p => p.Clone(context)).ToList();

            Expression = source.Expression?.CloneAstExpression(context);

            CompiledFunctionBody = source.CompiledFunctionBody?.Clone(context);

            AppendCodeItem(this, context);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(TypesList), TypesList);

            sb.PrintObjProp(n, nameof(Expression), Expression);
            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(TypesList), TypesList);

            sb.PrintShortObjProp(n, nameof(Expression), Expression);
            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(TypesList), TypesList);

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);
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

        private string NToHumanizedString()
        {
            var sb = new StringBuilder("expr:");

            if(Expression != null)
            {
                sb.Append($" {Expression.ToHumanizedLabel()}");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }
    }
}
