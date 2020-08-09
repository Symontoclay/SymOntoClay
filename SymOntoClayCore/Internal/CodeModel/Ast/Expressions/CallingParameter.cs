using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public class CallingParameter: AnnotatedItem
    {
        public AstExpression Name { get; set; }
        public AstExpression Value { get; set; }

        public bool IsNamed => Name != null;

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
        public CallingParameter Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public CallingParameter Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (CallingParameter)context[this];
            }

            var result = new CallingParameter();
            context[this] = result;

            result.Name = Name?.CloneAstExpression(context);
            result.Value = Value?.CloneAstExpression(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNamed)} = {IsNamed}");
            sb.PrintObjProp(n, nameof(Name), Name);
            sb.PrintObjProp(n, nameof(Value), Value);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNamed)} = {IsNamed}");
            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.PrintShortObjProp(n, nameof(Value), Value);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(IsNamed)} = {IsNamed}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
