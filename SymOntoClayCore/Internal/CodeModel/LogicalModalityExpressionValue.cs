using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalModalityExpressionValue : Value
    {
        /// <inheritdoc/>
        public override bool IsLogicalModalityExpressionValue => true;

        /// <inheritdoc/>
        public override LogicalModalityExpressionValue AsLogicalModalityExpressionValue => this;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.LogicalModalityExpressionValue;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new LogicalModalityExpressionValue();
            cloneContext[this] = result;

            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            throw new NotImplementedException();

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            throw new NotImplementedException();

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            throw new NotImplementedException();

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            throw new NotImplementedException();
        }
    }
}
