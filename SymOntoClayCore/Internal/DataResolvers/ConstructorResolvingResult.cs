using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ConstructorResolvingResult : IObjectToString
    {
        public Constructor Constructor { get; set; }
        public bool NeedTypeConversion { get; set; }
        public List<ParameterRank> ParametersRankMatrix { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(NeedTypeConversion)} = {NeedTypeConversion}");
            sb.PrintObjListProp(n, nameof(ParametersRankMatrix), ParametersRankMatrix);

            return sb.ToString();
        }
    }
}
