/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Var: CodeItem
    {
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Var;

        public Value Value { get; set; } = new NullValue();
        public List<StrongIdentifierValue> TypesList { get; set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem()
        {
            return Clone();
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
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
        public Var Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public Var Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Var)context[this];
            }

            var result = new Var();
            context[this] = result;

            result.AppendVar(this, context);

            return result;
        }

        protected void AppendVar(Var source, Dictionary<object, object> cloneContext)
        {
            Value = source.Value?.CloneValue(cloneContext);
            TypesList = source.TypesList?.Select(p => p.Clone(cloneContext)).ToList();

            AppendCodeItem(source, cloneContext);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Value), Value);
            sb.PrintObjListProp(n, nameof(TypesList), TypesList);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Value), Value);
            sb.PrintShortObjListProp(n, nameof(TypesList), TypesList);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.PrintBriefObjListProp(n, nameof(TypesList), TypesList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public string TypesListToHumanizedString()
        {
            if(TypesList.IsNullOrEmpty())
            {
                return "(any)";
            }

            return $"({string.Join(" | ", TypesList.Select(p => p.NameValue))})";
        }

        public string ToHumanizedString()
        {
            return $"{Name.NameValue}: {TypesListToHumanizedString()}";
        }
    }
}
