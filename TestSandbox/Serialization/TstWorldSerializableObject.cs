using NLog;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.Implementation;
using SymOntoClay.UnityAsset.Core.World;
using System;
using System.IO;
using System.Text;

namespace TestSandbox.Serialization
{
    public class TstWorldSerializableObject : IObjectToString
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public TstWorldSerializableObject(TstExternalSettings settings) 
        {
            _internal = new TstInternalWorldSerializableObject(settings);
        }

        public void Save(SerializationSettings settings)
        {
#if DEBUG
            _logger.Info($"settings = {settings}");
#endif

#if DEBUG
            if (!Directory.Exists(settings.Path))
            {
                Directory.CreateDirectory(settings.Path);
            }

            var targetPath = Path.Combine(settings.Path, settings.ImageName);

            _logger.Info($"targetPath = {targetPath}");

            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }

            Directory.CreateDirectory(targetPath);

            var serializationContext = new SerializationContext(targetPath);

            var serializer = new Serializer(serializationContext);

            serializer.Serialize(_internal);
#endif

            //throw new NotImplementedException("B0AD3C35-AAFD-4E6B-A6E8-665B2E672E03");
        }

        public void Load(SerializationSettings settings)
        {
#if DEBUG
            _logger.Info($"settings = {settings}");
#endif

#if DEBUG
            var targetPath = Path.Combine(settings.Path, settings.ImageName);

            _logger.Info($"targetPath = {targetPath}");

            var deserializationContext = new DeserializationContext(targetPath);

            var collector = new PreDeserializeItemsCollector(deserializationContext);

            collector.Run(_internal);

            var deserializer = new Deserializer(deserializationContext);

            _internal = deserializer.Deserialize<TstInternalWorldSerializableObject>();
#endif
        }

        public void SetSomeValue(int value)
        {
            _internal.SetSomeValue(value);
        }

        private TstInternalWorldSerializableObject _internal;

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }
        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.PrintObjProp(n, nameof(_internal), _internal);
            return sb.ToString();
        }
    }
}
