using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;
using System;
using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class StrongIdentifierPart : IObjectToString
    {
        public Token Token { get; set; }
        public StrongIdentifierLevel StrongIdentifierLevel { get; set; } = StrongIdentifierLevel.None;
        public List<StrongIdentifierPart> SubParts { get; set; } = new List<StrongIdentifierPart>();
        public int? Capacity { get; set; }
        public bool HasInfiniteCapacity { get; set; }

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
            var nextN = n + DisplayHelper.IndentationStep;
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(Token), Token);
            sb.AppendLine($"{spaces}{nameof(StrongIdentifierLevel)} = {StrongIdentifierLevel}");
            sb.PrintObjListProp(n, nameof(SubParts), SubParts);
            sb.AppendLine($"{spaces}{nameof(Capacity)} = {Capacity}");
            sb.AppendLine($"{spaces}{nameof(HasInfiniteCapacity)} = {HasInfiniteCapacity}");
            //sb.AppendLine($"{spaces}{nameof(Pos)} = {Pos}");
            //sb.AppendLine($"{spaces}{nameof(Line)} = {Line}");
            return sb.ToString();
        }
    }
}
