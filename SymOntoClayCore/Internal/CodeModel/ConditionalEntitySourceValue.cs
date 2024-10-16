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

using NLog;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Converters;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ConditionalEntitySourceValue : Value
    {
        public ConditionalEntitySourceValue(EntityConditionExpressionNode entityConditionExpression)
            : this(entityConditionExpression, null)
        {
        }

        public ConditionalEntitySourceValue(EntityConditionExpressionNode entityConditionExpression, StrongIdentifierValue name)
        {
            Expression = entityConditionExpression;
            Name = name;

            _isLogicalQueryGenerated = true;

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
        private bool _isLogicalQueryGenerated;
        private bool _isLongHashCodeRecalculatedWithEngineContext;
        private object _recalculatingLongHashCodeWithEngineContextLockObj = new object();

        private ConditionalEntitySourceValue()
        {
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.ConditionalEntitySourceValue;

        /// <inheritdoc/>
        public override bool IsConditionalEntitySourceValue => true;

        /// <inheritdoc/>
        public override ConditionalEntitySourceValue AsConditionalEntitySourceValue => this;

        public ConditionalEntityValue ConvertToConditionalEntityValue(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            lock(_recalculatingLongHashCodeWithEngineContextLockObj)
            {
                if (!_isLongHashCodeRecalculatedWithEngineContext)
                {
                    _isLongHashCodeRecalculatedWithEngineContext = true;

                    var options = new CheckDirtyOptions()
                    {
                        EngineContext= context,
                        LocalContext= localContext
                    };

                    CalculateLongHashCode(options);
                }
            }

            return new ConditionalEntityValue(Expression, LogicalQuery, Name, context, localContext);
        }

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        public override bool NullValueEquals()
        {
            return false;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return null;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.ConditionalEntityTypeName) };

            CheckDirtyOptions convertOptions = null;
            
            if(options == null)
            {
                convertOptions = new CheckDirtyOptions();
            }
            else
            {
                convertOptions = options.Clone();
            }

            var context = options?.EngineContext;

            if(context != null)
            {
                convertOptions.DontConvertConceptsToInhRelations = context.ServicesFactory.GetEntityConstraintsService().GetConstraintsList();

            }

            convertOptions.IgnoreStandaloneConceptsInNormalization= true;

            if (_isLogicalQueryGenerated)
            {
                LogicalQuery = ConverterEntityConditionExpressionToRuleInstance.Convert(Expression, convertOptions);
            }

            LogicalQuery.CheckDirty(convertOptions);

            return base.CalculateLongHashCode(options) ^ LongHashCodeWeights.BaseFunctionWeight ^ (Name?.GetLongHashCode(options) ?? 0) ^ LongHashCodeWeights.BaseParamWeight ^ LogicalQuery.GetLongHashCode(options);
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new ConditionalEntitySourceValue();
            context[this] = result;

            result.Name = Name?.Clone(context);
            result.Expression = Expression?.Clone(context);
            result.LogicalQuery = LogicalQuery?.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
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
        public override string ToHumanizedString(DebugHelperOptions options)
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
                sb.Append(DebugHelperForRuleInstance.ToString(LogicalQuery, options));
            }
            else
            {
                sb.Append("(");
                sb.Append(Expression.ToHumanizedString(options));
                sb.Append(")");
            }

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
            return new MonitoredHumanizedLabel()
            {
                Label = ToHumanizedString()
            };
        }
    }
}
