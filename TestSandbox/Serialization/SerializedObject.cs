using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using System.Text;
using System.Threading;

namespace TestSandbox.Serialization
{
    [SocSerialization]
    public partial class SerializedObject : IObjectToString
    {
        public int IntField;

        private object _lockObj = new object();

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
            sb.AppendLine($"{spaces}{nameof(IntField)} = {IntField}");
            return sb.ToString();
        }
    }
}
