using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CompoundHtnTaskCase: CodeItem
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.CompoundTaskCase;

        /// <inheritdoc/>
        public override bool IsCompoundTaskHtnCase => true;

        /// <inheritdoc/>
        public override CompoundHtnTaskCase AsCompoundTaskHtnCase => this;
        
        public List<CompoundHtnTaskCaseItem> Items { get; set; } = new List<CompoundHtnTaskCaseItem>();

        public AstExpression ConditionExpression { get; set; }

        public CompiledFunctionBody ConditionCompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CompoundHtnTaskCase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CompoundHtnTaskCase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CompoundHtnTaskCase)context[this];
            }

            var result = new CompoundHtnTaskCase();
            context[this] = result;
            
            result.Items = Items?.Select(p => p.Clone(context))?.ToList();

            result.ConditionExpression = ConditionExpression?.CloneAstExpression(context);

            result.ConditionCompiledFunctionBody = ConditionCompiledFunctionBody?.Clone(context);

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override ulong GetLongHashCode(CheckDirtyOptions options)
        {
            ConditionExpression.CheckDirty(options);

            var result = base.CalculateLongHashCode(options);

            if (!Items.IsNullOrEmpty())
            {
                foreach (var item in Items)
                {
                    result ^= item.GetLongHashCode(options);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!Items.IsNullOrEmpty())
            {
                foreach (var item in Items)
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

            sb.PrintObjListProp(n, nameof(Items), Items);

            sb.PrintObjProp(n, nameof(ConditionExpression), ConditionExpression);
            sb.PrintObjProp(n, nameof(ConditionCompiledFunctionBody), ConditionCompiledFunctionBody);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Items), Items);

            sb.PrintShortObjProp(n, nameof(ConditionExpression), ConditionExpression);
            sb.PrintShortObjProp(n, nameof(ConditionCompiledFunctionBody), ConditionCompiledFunctionBody);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Items), Items);

            sb.PrintBriefObjProp(n, nameof(ConditionExpression), ConditionExpression);
            sb.PrintBriefObjProp(n, nameof(ConditionCompiledFunctionBody), ConditionCompiledFunctionBody);

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

            sb.AppendLine("case");
            
            if(ConditionExpression != null)
            {
                throw new NotImplementedException("B8250B83-587D-46F9-A249-F540FC468C95");
            }

            sb.AppendLine("{");

            if (!Items.IsNullOrEmpty())
            {
                foreach (var item in Items)
                {
                    sb.AppendLine(item.ToHumanizedString(options));
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
            throw new NotImplementedException("BD5DB934-79CD-4EB2-B09B-4DA84050FB88");
        }
    }
}
