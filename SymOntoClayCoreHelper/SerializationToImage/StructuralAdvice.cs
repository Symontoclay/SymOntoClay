using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class StructuralAdvice : IObjectToString
    {
        public StructuralAdvice(KindOfSerializationStrategy kindOfSerializationStrategy, KindOfStructuralObject kindOfStructuralObject) 
        {
            KindOfSerializationStrategy = kindOfSerializationStrategy;
            KindOfStructuralObject = kindOfStructuralObject;
        }

        public KindOfSerializationStrategy KindOfSerializationStrategy { get; private set; }
        public KindOfStructuralObject KindOfStructuralObject { get; private set; }

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

            sb.AppendLine($"{spaces}{nameof(KindOfSerializationStrategy)} = {KindOfSerializationStrategy}");
            sb.AppendLine($"{spaces}{nameof(KindOfStructuralObject)} = {KindOfStructuralObject}");

            return sb.ToString();
        }
    }
}
