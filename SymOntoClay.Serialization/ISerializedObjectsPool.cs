namespace SymOntoClay.Serialization
{
    public interface ISerializedObjectsPool
    {
        bool IsSerialized(object obj);
        bool TryGetObjectPtr(object obj, out ObjectPtr objectPtr);
        void RegObjectPtr(object obj, ObjectPtr objectPtr);
    }
}
