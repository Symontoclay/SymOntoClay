using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class InlineTrigger : AnnotatedItem
    {
        public KindOfInlineTrigger Kind { get; set; } = KindOfInlineTrigger.Unknown;
        public KindOfSystemEventOfInlineTrigger KindOfSystemEvent { get; set; } = KindOfSystemEventOfInlineTrigger.Unknown;
        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        public CodeEntity CodeEntity { get; set; }

        public IndexedInlineTrigger Indexed { get; set; }

        public IndexedInlineTrigger GetIndexed(IEntityDictionary entityDictionary, ICompiler compiler)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertInlineTrigger(this, entityDictionary, compiler);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");
            sb.PrintObjListProp(n, nameof(Statements), Statements);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");

            sb.PrintShortObjListProp(n, nameof(Statements), Statements);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfSystemEvent)} = {KindOfSystemEvent}");

            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
