using System;

namespace SymOntoClay.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SocBasePlainObject : Attribute
    {
        public SocBasePlainObject(string basePlainObjectName)
        {
        }
    }
}
