using System;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class SocSerializableExternalSettings : Attribute
    {
        public SocSerializableExternalSettings(string instanceId)
        {
        }
    }
}
