/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RelationDescription: CodeItem
    {
        public RelationDescription()
        {
            TypeOfAccess = DefaultTypeOfAccess;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.RelationDescription;

        /// <inheritdoc/>
        public override bool IsRelationDescription => true;

        /// <inheritdoc/>
        public override RelationDescription AsRelationDescription => this;

        public List<RelationParameterDescription> Arguments { get; set; } = new List<RelationParameterDescription>();

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            var result = base.CalculateLongHashCode(options);

            if (Arguments.IsNullOrEmpty())
            {
                //_argumentsDict = new Dictionary<StrongIdentifierValue, FunctionArgumentInfo>();
            }
            else
            {
                //_argumentsDict = Arguments.ToDictionary(p => p.Name, p => p);

                foreach (var argument in Arguments)
                {
                    result ^= argument.GetLongConditionalHashCode(options);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            if (!Arguments.IsNullOrEmpty())
            {
                foreach (var item in Arguments)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public RelationDescription Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public RelationDescription Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RelationDescription)context[this];
            }

            var result = new RelationDescription();
            context[this] = result;

            result.Arguments = Arguments?.Select(p => p.Clone(context)).ToList();

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(Arguments), Arguments);

            //sb.PrintObjListProp(n, nameof(Statements), Statements);
            //sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Arguments), Arguments);

            //sb.PrintShortObjListProp(n, nameof(Statements), Statements);
            //sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Arguments), Arguments);

            //sb.PrintBriefObjListProp(n, nameof(Statements), Statements);
            //sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var sb = new StringBuilder($"{Name.NameValue} (");
            sb.Append(string.Join(", ", Arguments.Select(p => p.ToHumanizedString(options))));
            sb.Append(")");

            if(!InheritanceItems.IsNullOrEmpty())
            {
                sb.Append(" is ");
                sb.Append(string.Join(", ", InheritanceItems.Select(p => p.ToPartialHumanizedString(options))));
            }

            return sb.ToString();
        }
    }
}
