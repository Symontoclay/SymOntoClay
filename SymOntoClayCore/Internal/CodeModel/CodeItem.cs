using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class CodeItem: AnnotatedItem
    {
        public abstract KindOfCodeEntity Kind { get;}
        public StrongIdentifierValue Name { get; set; }
        public List<InheritanceItem> InheritanceItems { get; set; } = new List<InheritanceItem>();
        public CodeFile CodeFile { get; set; }
        public CodeItem ParentCodeEntity { get; set; }
        public List<CodeItem> SubItems { get; set; } = new List<CodeItem>();

        public virtual bool IsRuleInstance => false;
        public virtual RuleInstance AsRuleInstance => null;

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            Name?.CheckDirty(options);

            var result = base.CalculateLongHashCode(options);

            if(Name != null)
            {
                result ^= Name.GetLongHashCode();
            }

            return result;
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public abstract CodeItem CloneCodeItem();

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="cloneContext">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract CodeItem CloneCodeItem(Dictionary<object, object> cloneContext);

        protected void AppendCodeItem(CodeItem source, Dictionary<object, object> cloneContext)
        {
            Name = source.Name?.Clone(cloneContext);
            InheritanceItems = source.InheritanceItems?.Select(p => p.Clone(cloneContext)).ToList();

            CodeFile = source.CodeFile;
            ParentCodeEntity = source.ParentCodeEntity;
            SubItems = source.SubItems?.Select(p => p.CloneCodeItem(cloneContext)).ToList();

            AppendAnnotations(source, cloneContext);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);

            if (!InheritanceItems.IsNullOrEmpty())
            {
                foreach (var item in InheritanceItems)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (!SubItems.IsNullOrEmpty())
            {
                foreach (var item in SubItems)
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

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjListProp(n, nameof(InheritanceItems), InheritanceItems);

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
