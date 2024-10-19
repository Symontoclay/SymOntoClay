﻿namespace SymOntoClay.Serialization
{
    public interface ISerializer
    {
        void Serialize(object serializable);
        ObjectPtr GetSerializedObjectPtr(object obj);
        ObjectPtr GetSerializedObjectPtr(object obj, object settingsParameter,
#if SHOW_PARENT_OBJECT
            string parentObjInfo = ""
#endif
            );
    }
}
