using System;

namespace SymOntoClay.Serialization
{
    public interface IDeserializedExternalSettings
    {
        void RegExternalSettings(object settings, Type settingsType, Type holderType, string holderKey);
    }
}
