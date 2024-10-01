using System.Collections.Generic;

namespace SymOntoClay.Serialization.Implementation
{
    public class DeserializedObjectPool: IDeserializedObjectPool
    {
        private Dictionary<string, object> _deserializedObject = new Dictionary<string, object>();

        /// <inheritdoc/>
        public bool TryGetDeserializedObject(string instanceId, out object instance)
        {
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                instance = null;
                return true;
            }

            return _deserializedObject.TryGetValue(instanceId, out instance);
        }

        /// <inheritdoc/>
        public void RegDeserializedObject(string instanceId, object instance)
        {
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                return;
            }

            _deserializedObject[instanceId] = instance;
        }
    }
}
