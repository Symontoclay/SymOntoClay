using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class MutuallyExclusiveStatesSet: CodeItem
    {
        public MutuallyExclusiveStatesSet()
        {
            TypeOfAccess = DefaultTypeOfAccess;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.MutuallyExclusiveStatesSet;

        /// <inheritdoc/>
        public override bool IsMutuallyExclusiveStatesSet => true;

        /// <inheritdoc/>
        public override MutuallyExclusiveStatesSet AsMutuallyExclusiveStatesSet => this;

        public List<StrongIdentifierValue> StateNames { get; set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> cloneContext)
        {
            return Clone(cloneContext);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public MutuallyExclusiveStatesSet Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public MutuallyExclusiveStatesSet Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (MutuallyExclusiveStatesSet)context[this];
            }

            var result = new MutuallyExclusiveStatesSet();
            context[this] = result;

            result.StateNames = StateNames.Select(p => p.Clone(context)).ToList();

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintObjProp(n, nameof(Value), Value);
            sb.PrintObjListProp(n, nameof(StateNames), StateNames);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintShortObjProp(n, nameof(Value), Value);
            sb.PrintShortObjListProp(n, nameof(StateNames), StateNames);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.PrintBriefObjListProp(n, nameof(StateNames), StateNames);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        public string ToHumanizedString()
        {
            return $"states {{ {string.Join(",", StateNames.Select(p => p.NameValue))} }}";
        }
    }
}
