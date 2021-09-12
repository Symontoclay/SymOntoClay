using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class ActionDef : AnnotatedItem
    {
        public StrongIdentifierValue Name { get; set; }

        public CodeEntity CodeEntity { get; set; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
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

            result.Name = Name.Clone(context);
            //result.Range = Range?.Clone(context);
            //result.Constraint = Constraint?.Clone(context);
            //result.Values = Values?.Select(p => p.Clone(context)).ToList();
            //result.Operators = Operators?.Select(p => p.Clone(context)).ToList();

            result.CodeEntity = CodeEntity.Clone(context);

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);

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
        protected override ulong CalculateLongHashCode()
        {
            Name.CheckDirty();

            var result = base.CalculateLongHashCode() ^ Name.GetLongHashCode();

            //if (Range != null)
            //{
            //    Range.CheckDirty();

            //    result ^= Range.GetLongHashCode();
            //}

            //if (Constraint != null)
            //{
            //    Constraint.CheckDirty();
            //}

            //if (!Values.IsNullOrEmpty())
            //{
            //    foreach (var value in Values)
            //    {
            //        value.CheckDirty();

            //        result ^= value.GetLongHashCode();
            //    }
            //}

            //if (!Operators.IsNullOrEmpty())
            //{
            //    foreach (var op in Operators)
            //    {
            //        op.CheckDirty();

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

            sb.PrintObjProp(n, nameof(Name), Name);
            //sb.PrintObjProp(n, nameof(Range), Range);
            //sb.PrintObjProp(n, nameof(Constraint), Constraint);
            //sb.PrintObjListProp(n, nameof(Values), Values);
            //sb.PrintObjListProp(n, nameof(Operators), Operators);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Name), Name);
            //sb.PrintShortObjProp(n, nameof(Range), Range);
            //sb.PrintShortObjProp(n, nameof(Constraint), Constraint);
            //sb.PrintShortObjListProp(n, nameof(Values), Values);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Name), Name);
            //sb.PrintExisting(n, nameof(Range), Range);
            //sb.PrintExisting(n, nameof(Constraint), Constraint);
            //sb.PrintExistingList(n, nameof(Values), Values);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
