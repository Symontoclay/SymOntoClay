/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.RawStatements
{
    public class SetRawStatement: AnnotatedItem
    {
        public KindOfSetRawStatement KindOfSetRawStatement { get; set; } = KindOfSetRawStatement.Unknown;
        public StrongIdentifierValue FirstName { get; set; }
        public StrongIdentifierValue SecondName { get; set; }
        public Value Rank { get; set; }
        public bool HasNot { get; set; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public SetRawStatement Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public SetRawStatement Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (SetRawStatement)context[this];
            }

            var result = new SetRawStatement();
            context[this] = result;

            result.KindOfSetRawStatement = KindOfSetRawStatement;
            result.FirstName = FirstName?.Clone(context);
            result.SecondName = SecondName?.Clone(context);
            result.Rank = Rank?.CloneValue(context);
            result.HasNot = HasNot;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
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

            sb.AppendLine($"{spaces}{nameof(KindOfSetRawStatement)} = {KindOfSetRawStatement}");

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

            sb.AppendLine($"{spaces}{nameof(KindOfSetRawStatement)} = {KindOfSetRawStatement}");

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

            sb.AppendLine($"{spaces}{nameof(KindOfSetRawStatement)} = {KindOfSetRawStatement}");

            sb.PrintBriefObjProp(n, nameof(FirstName), FirstName);
            sb.PrintBriefObjProp(n, nameof(SecondName), SecondName);
            sb.PrintBriefObjProp(n, nameof(Rank), Rank);

            sb.AppendLine($"{spaces}{nameof(HasNot)} = {HasNot}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
