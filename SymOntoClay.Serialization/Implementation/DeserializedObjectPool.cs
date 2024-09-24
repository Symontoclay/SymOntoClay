using System.Collections.Generic;

namespace SymOntoClay.Serialization.Implementation
{
    public class DeserializedObjectPool: IDeserializedObjectPool
    {
        private Dictionary<string, object> _deserializedObject = new Dictionary<string, object>();

        /// <inheritdoc/>
        public bool TryGetDeserializedObject(string instanceId, out object instance)
        {
            return _deserializedObject.TryGetValue(instanceId, out instance);
        }

        /// <inheritdoc/>
        public void RegDeserializedObject(string instanceId, object instance)
        {
            _deserializedObject[instanceId] = instance;
        }
    }
}
