using System;

namespace SymOntoClay.Serialization
{
    public interface IDeserializedExternalSettings
    {
        void RegExternalSettings(object settings, Type holderType, string holderKey);
    }
}
