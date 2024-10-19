using System;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Obsolete("", true)]
    public class BasedOnSocNoSerializable : Attribute
    {
    }
}
