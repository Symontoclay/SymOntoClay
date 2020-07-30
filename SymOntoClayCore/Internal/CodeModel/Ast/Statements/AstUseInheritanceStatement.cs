using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Ast.Statements
{
    public class AstUseInheritanceStatement : AstStatement
    {
        public override KindOfAstStatement Kind => KindOfAstStatement.UseInheritance;

        public StrongIdentifierValue SubName { get; set; }
        public StrongIdentifierValue SuperName { get; set; }
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
            return CloneAstStatement(context);
        }

        /// <inheritdoc/>
        public override AstStatement CloneAstStatement(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AstStatement)context[this];
            }

            var result = new AstUseInheritanceStatement();
            context[this] = result;

            result.SubName = SubName.Clone(context);
            result.SuperName = SuperName.Clone(context);
            result.Rank = Rank.CloneValue(context);
            result.HasNot = HasNot;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(SubName), SubName);
            sb.PrintObjProp(n, nameof(SuperName), SuperName);
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

            sb.PrintShortObjProp(n, nameof(SubName), SubName);
            sb.PrintShortObjProp(n, nameof(SuperName), SuperName);
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

            sb.PrintBriefObjProp(n, nameof(SubName), SubName);
            sb.PrintBriefObjProp(n, nameof(SuperName), SuperName);
            sb.PrintBriefObjProp(n, nameof(Rank), Rank);

            sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
