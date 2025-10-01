using NLog.Conditions;
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
    public class CompoundHtnTaskCase: CompoundHtnTaskItemsSection
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.CompoundTaskCase;

        /// <inheritdoc/>
        public override bool IsCompoundTaskHtnCase => true;

        /// <inheritdoc/>
        public override CompoundHtnTaskCase AsCompoundTaskHtnCase => this;
        
        public LogicalExecutableExpression Condition { get; set; }
        public AstExpression ConditionExpression { get; set; }

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
            
            result.Condition = Condition?.Clone(context);
            result.ConditionExpression = ConditionExpression?.CloneAstExpression(context);

            FillUpCompoundHtnTaskItemsSection(result, context);

            return result;
        }

        /// <inheritdoc/>
        public override ulong GetLongHashCode(CheckDirtyOptions options)
        {
            Condition.CheckDirty(options);

            var result = base.CalculateLongHashCode(options);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Condition), Condition);
            sb.PrintObjProp(n, nameof(ConditionExpression), ConditionExpression);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Condition), Condition);
            sb.PrintShortObjProp(n, nameof(ConditionExpression), ConditionExpression);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Condition), Condition);
            sb.PrintBriefObjProp(n, nameof(ConditionExpression), ConditionExpression);

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

            sb.Append("case");
            
            if(Condition != null)
            {
                sb.Append($" {Condition.ToHumanizedString(options)}");
            }

            sb.AppendLine();

            sb.Append(ContentToHumanizedString(options));

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
