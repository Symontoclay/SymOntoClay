using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class CodeItemValue : Value
    {
        public CodeItemValue(CodeItem codeItem)
        {
            CodeItem = codeItem;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.CodeItemValue;

        /// <inheritdoc/>
        public override bool IsCodeItemValue => true;

        /// <inheritdoc/>
        public override CodeItemValue AsCodeItemValue => this;

        public CodeItem CodeItem { get; private set; }

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

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            CodeItem.CheckDirty(options);

            _builtInSuperTypes = new List<StrongIdentifierValue>() { CodeItem.Name };

            return base.CalculateLongHashCode(options) ^ CodeItem.GetLongHashCode();
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public CodeItemValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public CodeItemValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CodeItemValue)context[this];
            }

            var result = new CodeItemValue(CodeItem);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(CodeItem), CodeItem);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintShortObjProp(n, nameof(CodeItem), CodeItem);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintBriefObjProp(n, nameof(CodeItem), CodeItem);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{CodeItem.ToDbgString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return CodeItem.ToHumanizedString(options);
        }
    }
}
