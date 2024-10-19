using System;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    [Obsolete("", true)]
    public class SocSerializableActionKey : Attribute
    {
    }
}
