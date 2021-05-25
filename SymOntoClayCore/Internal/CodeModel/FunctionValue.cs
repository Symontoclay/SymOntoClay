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
    public abstract class FunctionValue : Value
    {
        public List<FunctionArgumentInfo> Arguments { get; set; } = new List<FunctionArgumentInfo>();

        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        public CodeEntity CodeEntity { get; set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.FunctionValue;

        /// <inheritdoc/>
        public override bool IsFunctionValue => true;

        /// <inheritdoc/>
        public override FunctionValue AsFunctionValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode();

            if(!Arguments.IsNullOrEmpty())
            {
                foreach(var argument in Arguments)
                {
                    result ^= argument.GetLongConditionalHashCode();
                }
            }

            result ^= CompiledFunctionBody.GetLongHashCode();

            return result;
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public abstract FunctionValue CloneFunctionValue();

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract FunctionValue CloneFunctionValue(Dictionary<object, object> context);

        protected void AppendFuction(FunctionValue source, Dictionary<object, object> context)
        {
            Arguments = source.Arguments?.Select(p => p.Clone(context)).ToList();

            Statements = source.Statements.Select(p => p.CloneAstStatement(context)).ToList();

            CompiledFunctionBody = source.CompiledFunctionBody.Clone(context);

            CodeEntity = source.CodeEntity.Clone(context);

            AppendAnnotations(this, context);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            if(!Arguments.IsNullOrEmpty())
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

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

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

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

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

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
