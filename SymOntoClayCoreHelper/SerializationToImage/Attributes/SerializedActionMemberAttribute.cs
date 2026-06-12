using System;

namespace SymOntoClay.CoreHelper.SerializationToImage.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class SerializedActionMemberAttribute: Attribute
    {
        public SerializedActionMemberAttribute(string keyParameterName, int index = 0)
        {
            KeyParameterName = keyParameterName;
            Index = index;
        }

        public string KeyParameterName { get; }

        public int Index { get; }
    }
}
