using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SerializableAction : Attribute, IObjectToString
    {
        public SerializableAction()
        {
        }

        public SerializableAction(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

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

        private string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");

            return sb.ToString();
        }
    }
}
