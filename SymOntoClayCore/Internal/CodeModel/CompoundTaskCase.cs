using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System.Text;
using SymOntoClay.Common.CollectionsHelpers;
using System.Linq;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CompoundTaskCase: CodeItem
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.CompoundTaskCase;

        /// <inheritdoc/>
        public override bool IsCompoundTaskCase => true;

        /// <inheritdoc/>
        public override CompoundTaskCase AsCompoundTaskCase => this;
        
        public List<CompoundTaskCaseItem> Items { get; set; } = new List<CompoundTaskCaseItem>();

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CompoundTaskCase Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public CompoundTaskCase Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CompoundTaskCase)context[this];
            }

            var result = new CompoundTaskCase();
            context[this] = result;
            
            result.Items = Items?.Select(p => p.Clone(context))?.ToList();

            result.AppendCodeItem(this, context);

            return result;
        }

        public ulong GetLongHashCode(CheckDirtyOptions options)
        {
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

        public void DiscoverAllAnnotations(IList<Annotation> result)
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

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Items), Items);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Items), Items);

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
