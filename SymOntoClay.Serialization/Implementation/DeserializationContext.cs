using NLog;
using System.Collections.Generic;

namespace SymOntoClay.Serialization.Implementation
{
    public class DeserializationContext : IDeserializationContext
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public DeserializationContext(string dirName)
            : this(dirName, dirName, new DeserializedObjectPool())
        {
        }

        public DeserializationContext(string heapDirName, string rootDirName, IDeserializedObjectPool deserializedObjectPool)
        {
#if DEBUG
            _logger.Info($"heapDirName = {heapDirName}");
            _logger.Info($"rootDirName = {rootDirName}");
#endif

            _heapDirName = heapDirName;
            _rootDirName = rootDirName;

            _deserializedObjectPool = deserializedObjectPool;
        }

        private string _heapDirName;
        private string _rootDirName;

        private IDeserializedObjectPool _deserializedObjectPool;

        /// <inheritdoc/>
        public string HeapDirName => _heapDirName;

        /// <inheritdoc/>
        public string RootDirName => _rootDirName;

        /// <inheritdoc/>
        public bool TryGetDeserializedObject(string instanceId, out object instance)
        {
            return _deserializedObjectPool.TryGetDeserializedObject(instanceId, out instance);
        }

        /// <inheritdoc/>
        public void RegDeserializedObject(string instanceId, object instance)
        {
            _deserializedObjectPool.RegDeserializedObject(instanceId, instance);
        }
    }
}
