namespace SymOntoClay.Serialization
{
    public interface ISerializedObjectsPool
    {
        bool IsSerialized(object obj);
        bool TryGetObjectPtr(object obj, out ObjectPtr objectPtr);
        bool TryGetObject(ObjectPtr objectPtr, out object obj);
        void RegObjectPtr(object obj, ObjectPtr objectPtr);
    }
}
