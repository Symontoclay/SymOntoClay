namespace SymOntoClay.Serialization
{
    public interface ISerializationContext
    {
        string DirName { get; }
        bool IsSerialized(object obj);
        bool TryGetObjectPtr(object obj, out ObjectPtr objectPtr);
        void RegObjectPtr(object obj, ObjectPtr objectPtr);
    }
}
