using SymOntoClay.Common.DebugHelpers;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class BaseHtnTask : CodeItem
    {
        /// <inheritdoc/>
        public override bool IsBaseHtnTask => true;

        /// <inheritdoc/>
        public override BaseHtnTask AsBaseHtnTask => this;

        public abstract BaseHtnTask CloneBaseTask(Dictionary<object, object> context);

        public abstract KindOfTask KindOfTask { get; }

        public LogicalExecutableExpression Precondition { get; set; }

        protected void AppendBaseHtnTask(BaseHtnTask source, Dictionary<object, object> context)
        {
            Precondition = source.Precondition?.Clone(context);

            AppendCodeItem(source, context);
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            Precondition.CheckDirty(options);

            var result = base.CalculateLongHashCode(options);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");
            sb.PrintObjProp(n, nameof(Precondition), Precondition);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");
            sb.PrintShortObjProp(n, nameof(Precondition), Precondition);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfTask)} = {KindOfTask}");
            sb.PrintBriefObjProp(n, nameof(Precondition), Precondition);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
