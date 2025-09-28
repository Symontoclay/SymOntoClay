using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseCompoundHtnTask: BaseHtnTask
    {
        /// <inheritdoc/>
        public override bool IsBaseCompoundHtnTask => true;

        /// <inheritdoc/>
        public override BaseCompoundHtnTask AsBaseCompoundHtnTask => this;

        public CompoundHtnTaskBefore Before { get; set; }
        public CompoundHtnTaskAfter After { get; set; }

        public List<CompoundHtnTaskBackground> Backgrounds { get; set; } = new List<CompoundHtnTaskBackground>();

        public List<CompoundHtnTaskCase> Cases { get; set; } = new List<CompoundHtnTaskCase>();

        public abstract BaseCompoundHtnTask CloneBaseCompoundTask(Dictionary<object, object> context);

        protected void AppendBaseCompoundTask(BaseCompoundHtnTask source, Dictionary<object, object> context)
        {
            Before = source.Before?.Clone(context);
            After = source.After?.Clone(context);
            Cases = source.Cases?.Select(p => p.Clone(context))?.ToList();
            Backgrounds = source.Backgrounds.Select(p => p.Clone(context))?.ToList();

            AppendBaseHtnTask(source, context);
        }
        
        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            if (!Cases.IsNullOrEmpty())
            {
                foreach (var item in Cases)
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

            Before?.DiscoverAllAnnotations(result);
            After?.DiscoverAllAnnotations(result);

            if (!Cases.IsNullOrEmpty())
            {
                foreach (var item in Cases)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (!Backgrounds.IsNullOrEmpty())
            {
                foreach (var item in Backgrounds)
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

            sb.PrintObjProp(n, nameof(Before), Before);
            sb.PrintObjProp(n, nameof(After), After);
            sb.PrintObjListProp(n, nameof(Cases), Cases); 
            sb.PrintObjListProp(n, nameof(Backgrounds), Backgrounds);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Before), Before);
            sb.PrintShortObjProp(n, nameof(After), After);
            sb.PrintShortObjListProp(n, nameof(Cases), Cases);
            sb.PrintShortObjListProp(n, nameof(Backgrounds), Backgrounds);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Before), Before);
            sb.PrintBriefObjProp(n, nameof(After), After);
            sb.PrintBriefObjListProp(n, nameof(Cases), Cases);
            sb.PrintBriefObjListProp(n, nameof(Backgrounds), Backgrounds);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
