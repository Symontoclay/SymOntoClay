using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ConditionalEntitySourceValue : Value
    {
        public ConditionalEntitySourceValue(EntityConditionExpressionNode entityConditionExpression, StrongIdentifierValue name)
        {
            Expression = entityConditionExpression;
            Name = name;

            CheckName();
        }

        public ConditionalEntitySourceValue(RuleInstance logicalQuery, StrongIdentifierValue name)
        {
            LogicalQuery = logicalQuery;
            Name = name;

            CheckName();
        }

        private void CheckName()
        {
            if(Name == null)
            {
                Name = new StrongIdentifierValue();
            }
        }

        public EntityConditionExpressionNode Expression { get; private set; }
        public StrongIdentifierValue Name { get; private set; }
        public RuleInstance LogicalQuery { get; private set; }

        private ConditionalEntitySourceValue()
        {
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ConditionalEntitySourceValue;

        /// <inheritdoc/>
        public override bool IsConditionalEntitySourceValue => true;

        /// <inheritdoc/>
        public override ConditionalEntitySourceValue AsConditionalEntitySourceValue => this;

        public ConditionalEntityValue ConvertToConditionalEntityValue(IEngineContext context, LocalCodeExecutionContext localContext)
        {
            return new ConditionalEntityValue(Expression, LogicalQuery, Name, context, localContext);
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            if (LogicalQuery == null)
            {
                LogicalQuery = ConvertorEntityConditionExpressionToRuleInstance.Convert(Expression, options);
            }

            LogicalQuery.CheckDirty(options);

            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseFunctionWeight ^ (Name?.GetLongHashCode(options) ?? 0) ^ LongHashCodeWeights.BaseParamWeight ^ LogicalQuery.GetLongHashCode(options);
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

            var result = new ConditionalEntitySourceValue();
            cloneContext[this] = result;

            result.Name = Name?.Clone(cloneContext);
            result.Expression = Expression?.Clone(cloneContext);
            result.LogicalQuery = LogicalQuery?.Clone(cloneContext);

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Expression), Expression);
            sb.PrintObjProp(n, nameof(LogicalQuery), LogicalQuery);
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Expression), Expression);
            sb.PrintShortObjProp(n, nameof(LogicalQuery), LogicalQuery);
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Expression), Expression);
            sb.PrintBriefObjProp(n, nameof(LogicalQuery), LogicalQuery);
            sb.PrintBriefObjProp(n, nameof(Name), Name);

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
        public override string ToHumanizedString()
        {
            var sb = new StringBuilder();

            if (Name != null && !Name.IsEmpty)
            {
                sb.Append(Name.NameValue);
            }
            else
            {
                sb.Append("#@");
            }

            if (Expression == null)
            {
                sb.Append(DebugHelperForRuleInstance.ToString(LogicalQuery));
            }
            else
            {
                sb.Append("(");
                sb.Append(Expression.ToHumanizedString());
                sb.Append(")");
            }

            return sb.ToString();
        }
    }
}
