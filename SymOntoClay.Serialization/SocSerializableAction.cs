using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SocSerializableAction : Attribute
    {
        public string Id { get; set; }
    }
}
