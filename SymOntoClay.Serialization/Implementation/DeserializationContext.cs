using NLog;
using System.Collections.Generic;

namespace SymOntoClay.Serialization.Implementation
{
    public class DeserializationContext : IDeserializationContext
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public DeserializationContext(string dirName)
        {
            _dirName = dirName;

#if DEBUG
            _logger.Info($"_dirName = {_dirName}");
#endif
        }

        private string _dirName;
        private DeserializedObjectPool _deserializedObjectPool = new DeserializedObjectPool();

        /// <inheritdoc/>
        public string HeapDirName => _dirName;

        /// <inheritdoc/>
        public string RootDirName => _dirName;

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
