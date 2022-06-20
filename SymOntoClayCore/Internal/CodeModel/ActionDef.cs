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

using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ActionDef : CodeItem
    {
        public ActionDef()
        {
            TypeOfAccess = CodeItem.DefaultTypeOfAccess;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Action;

        /// <inheritdoc/>
        public override bool IsAction => true;

        /// <inheritdoc/>
        public override ActionDef AsAction => this;

        public void AddAliasRange(List<StrongIdentifierValue> aliasList)
        {
            _aliasesList.AddRange(aliasList);
        }

        public IList<StrongIdentifierValue> NamesList => _namesList;

        private List<StrongIdentifierValue> _namesList;
        private List<StrongIdentifierValue> _aliasesList = new List<StrongIdentifierValue>();

        public void AddOperator(Operator op)
        {
            if(_operatorsList.Contains(op))
            {
                return;
            }

            _operatorsList.Add(op);
        }

        private List<Operator> _operatorsList = new List<Operator>();

        public IList<Operator> Operators => _operatorsList;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public ActionDef Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public ActionDef Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (ActionDef)context[this];
            }

            var result = new ActionDef();
            context[this] = result;

            result._aliasesList = _aliasesList?.Select(p => p.Clone(context)).ToList();
            result._namesList = _namesList?.Select(p => p.Clone(context)).ToList();
            
            //result.Range = Range?.Clone(context);
            //result.Constraint = Constraint?.Clone(context);
            //result.Values = Values?.Select(p => p.Clone(context)).ToList();
            result._operatorsList = _operatorsList?.Select(p => p.Clone(context)).ToList();

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            //if (!Values.IsNullOrEmpty())
            //{
            //    foreach (var value in Values)
            //    {
            //        value.DiscoverAllAnnotations(result);
            //    }
            //}

            if (!Operators.IsNullOrEmpty())
            {
                foreach (var op in Operators)
                {
                    op.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            foreach(var alias in _aliasesList)
            {
                alias.CheckDirty(options);
            }

            var result = base.CalculateLongHashCode(options);

            _namesList = new List<StrongIdentifierValue>() { Name };
            _namesList.AddRange(_aliasesList);

            if (!Operators.IsNullOrEmpty())
            {
                foreach (var op in Operators)
                {
                    op.CheckDirty(options);

                    result ^= op.GetLongHashCode();
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintObjProp(n, nameof(Range), Range);
            //sb.PrintObjProp(n, nameof(Constraint), Constraint);
            sb.PrintObjListProp(n, nameof(NamesList), NamesList);
            //sb.PrintObjListProp(n, nameof(Operators), Operators);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintShortObjProp(n, nameof(Range), Range);
            //sb.PrintShortObjProp(n, nameof(Constraint), Constraint);
            sb.PrintShortObjListProp(n, nameof(NamesList), NamesList);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintExisting(n, nameof(Range), Range);
            //sb.PrintExisting(n, nameof(Constraint), Constraint);
            sb.PrintExistingList(n, nameof(NamesList), NamesList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
