using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using System;
using System.Text;

namespace TestSandbox.SerializationToImage
{
    [WorldRootAttribute]
    [SerializeOnlyExplicitlySerializableMembersAttribute]
    public class TstWorldContext : IObjectToString
    {


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

            return sb.ToString();
        }
    }
}
