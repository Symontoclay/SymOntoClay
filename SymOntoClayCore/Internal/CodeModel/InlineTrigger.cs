/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SymOntoClay.CoreHelper.CollectionsHelpers;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class InlineTrigger : AnnotatedItem
    {
        public KindOfInlineTrigger Kind { get; set; } = KindOfInlineTrigger.Unknown;
        public KindOfSystemEventOfInlineTrigger KindOfSystemEvent { get; set; } = KindOfSystemEventOfInlineTrigger.Unknown;
        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        public CodeEntity CodeEntity { get; set; }

        public IndexedInlineTrigger Indexed { get; set; }

        public IndexedInlineTrigger GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertInlineTrigger(this, mainStorageContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => Indexed;

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertInlineTrigger(this, mainStorageContext, convertingContext);
            }

            return Indexed;
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
        public InlineTrigger Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public InlineTrigger Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (InlineTrigger)context[this];
            }

            var result = new InlineTrigger();
            context[this] = result;

            result.Kind = Kind;
            result.KindOfSystemEvent = KindOfSystemEvent;
            result.Statements = Statements.Select(p => p.CloneAstStatement(context)).ToList();
            result.CodeEntity = CodeEntity.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!Statements.IsNullOrEmpty())
            {
                foreach (var item in Statements)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
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
