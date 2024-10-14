using System;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class SocSerializableActionMember : Attribute
    {
        public SocSerializableActionMember(string keyParameterName, int index = 0)
        {
        }
    }
}
