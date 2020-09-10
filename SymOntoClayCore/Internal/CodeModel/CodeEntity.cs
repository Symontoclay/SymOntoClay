using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CodeEntity : AnnotatedItem
    {
        public KindOfCodeEntity Kind { get; set; } = KindOfCodeEntity.Unknown;
        public StrongIdentifierValue Name { get; set; }
        public List<InheritanceItem> InheritanceItems { get; set; } = new List<InheritanceItem>();
        public RuleInstance RuleInstance { get; set; }
        public InlineTrigger InlineTrigger { get; set; }
        public Operator Operator { get; set; }
        public Channel Channel { get; set; }

        public CodeFile CodeFile { get; set; }
        public CodeEntity ParentCodeEntity { get; set; }
        public List<CodeEntity> SubItems { get; set; } = new List<CodeEntity>();

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => null;

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext)
        {
            return null;
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            return null;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public CodeEntity Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public CodeEntity Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CodeEntity)context[this];
            }

            var result = new CodeEntity();
            context[this] = result;

            result.Kind = Kind;
            result.Name = Name?.Clone(context);
            result.InheritanceItems = InheritanceItems?.Select(p => p.Clone(context)).ToList();
            result.RuleInstance = RuleInstance?.Clone(context);
            result.InlineTrigger = InlineTrigger?.Clone(context);
            result.Operator = Operator?.Clone(context);
            result.Channel = Channel?.Clone(context);

            result.CodeFile = CodeFile;
            result.ParentCodeEntity = ParentCodeEntity;
            result.SubItems = SubItems?.Select(p => p.Clone(context)).ToList();

            result.AppendAnnotations(this, context);

            return result;            
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            /*
                     public KindOfCodeEntity Kind { get; set; } = KindOfCodeEntity.Unknown;
        public StrongIdentifierValue Name { get; set; }
        public List<InheritanceItem> InheritanceItems { get; set; } = new List<InheritanceItem>();
        public RuleInstance RuleInstance { get; set; }
        public InlineTrigger InlineTrigger { get; set; }
        public Operator Operator { get; set; }
        public Channel Channel { get; set; }

        public CodeFile CodeFile { get; set; }
        public CodeEntity ParentCodeEntity { get; set; }
        public List<CodeEntity> SubItems { get; set; } = new List<CodeEntity>();
            */

            d

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjListProp(n, nameof(InheritanceItems), InheritanceItems);
            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.PrintObjProp(n, nameof(InlineTrigger), InlineTrigger);
            sb.PrintObjProp(n, nameof(Operator), Operator);
            sb.PrintObjProp(n, nameof(Channel), Channel);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintObjListProp(n, nameof(SubItems), SubItems);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjListProp(n, nameof(InheritanceItems), InheritanceItems);
            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);
            sb.PrintShortObjProp(n, nameof(InlineTrigger), InlineTrigger);
            sb.PrintShortObjProp(n, nameof(Operator), Operator);
            sb.PrintShortObjProp(n, nameof(Channel), Channel);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintShortObjListProp(n, nameof(SubItems), SubItems);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
