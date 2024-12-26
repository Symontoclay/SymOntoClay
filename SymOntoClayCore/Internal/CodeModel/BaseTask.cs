using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseTask : CodeItem
    {
        /// <inheritdoc/>
        public override bool IsBaseTask => true;

        /// <inheritdoc/>
        public override BaseTask AsBaseTask => this;

        public abstract BaseTask CloneBaseTask(Dictionary<object, object> context);

        public abstract KindOfTask KindOfTask { get; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
