/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
            FirstCoordinate?.DiscoverAllAnnotations(result);
            SecondCoordinate?.DiscoverAllAnnotations(result);
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
