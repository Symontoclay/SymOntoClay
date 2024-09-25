using System.Collections.Generic;

namespace SymOntoClay.Serialization.Implementation
{
    public class SerializedObjectsPool: ISerializedObjectsPool
    {
        private Dictionary<object, ObjectPtr> _serializedObjects = new Dictionary<object, ObjectPtr>();

        /// <inheritdoc/>
        public bool IsSerialized(object obj)
        {
            return _serializedObjects.ContainsKey(obj);
        }

        /// <inheritdoc/>
        public bool TryGetObjectPtr(object obj, out ObjectPtr objectPtr)
        {
            return _serializedObjects.TryGetValue(obj, out objectPtr);
        }

        /// <inheritdoc/>
        public void RegObjectPtr(object obj, ObjectPtr objectPtr)
        {
            _serializedObjects[obj] = objectPtr;
        }
    }
}
