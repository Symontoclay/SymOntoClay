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

        public List<CompoundHtnTaskCase> Cases { get; set; } = new List<CompoundHtnTaskCase>();

        public abstract BaseCompoundHtnTask CloneBaseCompoundTask(Dictionary<object, object> context);

        protected void AppendBaseCompoundTask(BaseCompoundHtnTask source, Dictionary<object, object> context)
        {
            Cases = source.Cases?.Select(p => p.Clone(context))?.ToList();

            AppendCodeItem(source, context);
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

            if (!Cases.IsNullOrEmpty())
            {
                foreach (var item in Cases)
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

            sb.PrintObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Cases), Cases);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
