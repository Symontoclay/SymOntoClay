using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SocSerializableAction : Attribute
    {
        public SocSerializableAction(string id) 
        {
            Id = id;
        }

        public string Id { get; private set; }
    }
}
