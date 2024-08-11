using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class SocSerializableActionKey : Attribute
    {
    }
}
