using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class StateDef : CodeItem
    {
        public StateDef()
        {
            TypeOfAccess = TypeOfAccess.Public;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.State;

        /// <inheritdoc/>
        public override bool IsState => true;

        /// <inheritdoc/>
        public override StateDef AsState => this;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public StateDef Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public StateDef Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (StateDef)context[this];
            }

            var result = new StateDef();
            context[this] = result;

            //result._aliasesList = _aliasesList?.Select(p => p.Clone(context)).ToList();
            //result._namesList = _namesList?.Select(p => p.Clone(context)).ToList();

            //result.Range = Range?.Clone(context);
            //result.Constraint = Constraint?.Clone(context);
            //result.Values = Values?.Select(p => p.Clone(context)).ToList();
            //result._operatorsList = _operatorsList?.Select(p => p.Clone(context)).ToList();

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            //if (!Values.IsNullOrEmpty())
            //{
            //    foreach (var value in Values)
            //    {
            //        value.DiscoverAllAnnotations(result);
            //    }
            //}

            //if (!Operators.IsNullOrEmpty())
            //{
            //    foreach (var op in Operators)
            //    {
            //        op.DiscoverAllAnnotations(result);
            //    }
            //}
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            //foreach (var alias in _aliasesList)
            //{
            //    alias.CheckDirty(options);
            //}

            var result = base.CalculateLongHashCode(options);

            //_namesList = new List<StrongIdentifierValue>() { Name };
            //_namesList.AddRange(_aliasesList);

            //if (!Operators.IsNullOrEmpty())
            //{
            //    foreach (var op in Operators)
            //    {
            //        op.CheckDirty(options);

            //        result ^= op.GetLongHashCode();
            //    }
            //}

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintObjProp(n, nameof(Range), Range);
            //sb.PrintObjProp(n, nameof(Constraint), Constraint);
            //sb.PrintObjListProp(n, nameof(NamesList), NamesList);
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
            //sb.PrintShortObjListProp(n, nameof(NamesList), NamesList);

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
            //sb.PrintExistingList(n, nameof(NamesList), NamesList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
