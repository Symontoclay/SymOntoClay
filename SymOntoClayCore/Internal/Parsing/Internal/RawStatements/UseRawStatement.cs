using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.RawStatements
{
    public class UseRawStatement: AnnotatedItem
    {
        public KindOfUseRawStatement KindOfUseRawStatement { get; set; } = KindOfUseRawStatement.Unknown;
        public StrongIdentifierValue FirstName { get; set; }
        public StrongIdentifierValue SecondName { get; set; }
        public Value Rank { get; set; }
        public bool HasNot { get; set; }

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
        public UseRawStatement Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public UseRawStatement Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (UseRawStatement)context[this];
            }

            var result = new UseRawStatement();
            context[this] = result;

            result.KindOfUseRawStatement = KindOfUseRawStatement;
            result.FirstName = FirstName?.Clone(context);
            result.SecondName = SecondName?.Clone(context);
            result.Rank = Rank?.CloneValue(context);
            result.HasNot = HasNot;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            FirstName?.DiscoverAllAnnotations(result);
            SecondName?.DiscoverAllAnnotations(result);
            Rank?.DiscoverAllAnnotations(result);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfUseRawStatement)} = {KindOfUseRawStatement}");

            sb.PrintObjProp(n, nameof(FirstName), FirstName);
            sb.PrintObjProp(n, nameof(SecondName), SecondName);
            sb.PrintObjProp(n, nameof(Rank), Rank);

            sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfUseRawStatement)} = {KindOfUseRawStatement}");

            sb.PrintShortObjProp(n, nameof(FirstName), FirstName);
            sb.PrintShortObjProp(n, nameof(SecondName), SecondName);
            sb.PrintShortObjProp(n, nameof(Rank), Rank);

            sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfUseRawStatement)} = {KindOfUseRawStatement}");

            sb.PrintBriefObjProp(n, nameof(FirstName), FirstName);
            sb.PrintBriefObjProp(n, nameof(SecondName), SecondName);
            sb.PrintBriefObjProp(n, nameof(Rank), Rank);

            sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
