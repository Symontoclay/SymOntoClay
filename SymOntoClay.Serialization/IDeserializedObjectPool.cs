namespace SymOntoClay.Serialization
{
    public interface IDeserializedObjectPool
    {
        bool TryGetDeserializedObject(string instanceId, out object instance);
        void RegDeserializedObject(string instanceId, object instance);
    }
}
