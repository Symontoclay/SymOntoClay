using System.Collections.Generic;
using System.IO;
using System;

namespace SymOntoClay.Serialization.Implementation
{
    public class SerializationContext : ISerializationContext
    {
        public SerializationContext(string dirName)
            : this(dirName, dirName, true, new SerializedObjectsPool())
        {
        }

        public SerializationContext(string heapDirName, string rootDirName, ISerializedObjectsPool serializedObjectsPool)
            : this(heapDirName, rootDirName, true, serializedObjectsPool)
        {
        }

        public SerializationContext(string heapDirName, string rootDirName, bool checkDirectories, ISerializedObjectsPool serializedObjectsPool)
        {
            _heapDirName = heapDirName;
            _rootDirName = rootDirName;

            if(checkDirectories)
            {
                CheckDirectory(heapDirName);
                CheckDirectory(rootDirName);
            }

            _serializedObjectsPool = serializedObjectsPool;
        }

        private void CheckDirectory(string dirName)
        {
            if (Directory.Exists(dirName))
            {
                Directory.Delete(dirName, true);
            }

            Directory.CreateDirectory(dirName);
        }

        private string _heapDirName;
        private string _rootDirName;
        private ISerializedObjectsPool _serializedObjectsPool;

        /// <inheritdoc/>
        public string HeapDirName => _heapDirName;

        /// <inheritdoc/>
        public string RootDirName => _rootDirName;

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
