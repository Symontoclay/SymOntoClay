using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BasePrimitiveHtnTask : BaseHtnTask
    {
        /// <inheritdoc/>
        public override bool IsBasePrimitiveHtnTask => true;

        /// <inheritdoc/>
        public override BasePrimitiveHtnTask AsBasePrimitiveHtnTask => this;

        public abstract KindOfPrimitiveTask KindOfPrimitiveTask { get; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfPrimitiveTask)} = {KindOfPrimitiveTask}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfPrimitiveTask)} = {KindOfPrimitiveTask}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfPrimitiveTask)} = {KindOfPrimitiveTask}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
