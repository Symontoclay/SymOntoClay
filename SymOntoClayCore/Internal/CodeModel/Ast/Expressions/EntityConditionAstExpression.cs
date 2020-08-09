using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Expressions
{
    public class EntityConditionAstExpression: AstExpression
    {
        /// <inheritdoc/>
        public override KindOfAstExpression Kind => KindOfAstExpression.EntityCondition;

        public KindOfEntityConditionAstExpression KindOfEntityConditionAstExpression { get; set; }

        public StrongIdentifierValue Name { get; set; }
        public AstExpression FirstCoordinate { get; set; }
        public AstExpression SecondCoordinate { get; set; }

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
            return CloneAstExpression(context);
        }

        /// <inheritdoc/>
        public override AstExpression CloneAstExpression(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstExpression)context[this];
            }

            var result = new EntityConditionAstExpression();
            context[this] = result;

            result.KindOfEntityConditionAstExpression = KindOfEntityConditionAstExpression;
            result.Name = Name.Clone(context);

            result.FirstCoordinate = FirstCoordinate?.CloneAstExpression(context);
            result.SecondCoordinate = SecondCoordinate?.CloneAstExpression(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfEntityConditionAstExpression)} = {KindOfEntityConditionAstExpression}");
            
            sb.PrintObjProp(n, nameof(Name), Name);
            sb.PrintObjProp(n, nameof(FirstCoordinate), FirstCoordinate);
            sb.PrintObjProp(n, nameof(SecondCoordinate), SecondCoordinate);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfEntityConditionAstExpression)} = {KindOfEntityConditionAstExpression}");

            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.PrintShortObjProp(n, nameof(FirstCoordinate), FirstCoordinate);
            sb.PrintShortObjProp(n, nameof(SecondCoordinate), SecondCoordinate);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfEntityConditionAstExpression)} = {KindOfEntityConditionAstExpression}");

            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.PrintBriefObjProp(n, nameof(FirstCoordinate), FirstCoordinate);
            sb.PrintBriefObjProp(n, nameof(SecondCoordinate), SecondCoordinate);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }
    }
}
