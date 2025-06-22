using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Converters;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ParameterRank : IObjectToString
    {
        public uint Distance { get; set; }
        public bool NeedTypeConversion { get; set; }
        public TypeFitCheckingResult TypeFitCheckingResult { get; set; }
        public StrongIdentifierValue ParameterActualName { get; set; }

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Distance)} = {Distance}");
            sb.AppendLine($"{spaces}{nameof(NeedTypeConversion)} = {NeedTypeConversion}");
            sb.PrintObjProp(n, nameof(TypeFitCheckingResult), TypeFitCheckingResult);
            sb.PrintObjProp(n, nameof(ParameterActualName), ParameterActualName);
            return sb.ToString();
        }
    }
}
