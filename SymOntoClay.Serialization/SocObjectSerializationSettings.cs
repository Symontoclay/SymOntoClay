using System;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class SocObjectSerializationSettings : Attribute
    {
        public SocObjectSerializationSettings(string settingsParameterName)
        {
        }
    }
}
