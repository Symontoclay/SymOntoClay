using System.Collections.Generic;
using System.IO;
using System;

namespace SymOntoClay.Serialization.Implementation
{
    public class SerializationContext : ISerializationContext
    {
        public SerializationContext(string dirName)
        {
            //_dirName = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString("D"));

            _dirName = dirName;

            if(Directory.Exists(_dirName))
            {
                Directory.Delete(_dirName, true);
            }

            Directory.CreateDirectory(_dirName);
        }

        private string _dirName;
        private SerializedObjectsPool _serializedObjectsPool = new SerializedObjectsPool();

        /// <inheritdoc/>
        public string DirName => _dirName;

        /// <inheritdoc/>
        public bool IsSerialized(object obj)
        {
            return _serializedObjectsPool.IsSerialized(obj);
        }

        /// <inheritdoc/>
        public bool TryGetObjectPtr(object obj, out ObjectPtr objectPtr)
        {
            return _serializedObjectsPool.TryGetObjectPtr(obj, out objectPtr);
        }

        /// <inheritdoc/>
        public void RegObjectPtr(object obj, ObjectPtr objectPtr)
        {
            _serializedObjectsPool.RegObjectPtr(obj, objectPtr);
        }
    }
}
