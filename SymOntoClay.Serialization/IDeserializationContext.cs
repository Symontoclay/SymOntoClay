namespace SymOntoClay.Serialization
{
    public interface IDeserializationContext
    {
        string DirName { get; }
        bool TryGetDeserializedObject(string instanceId, out object instance);
        void RegDeserializedObject(string instanceId, object instance);
    }
}
