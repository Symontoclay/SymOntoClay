/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
