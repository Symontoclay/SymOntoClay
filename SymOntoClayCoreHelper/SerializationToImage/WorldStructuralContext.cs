using SymOntoClay.CoreHelper.SerializationToImage.Attributes;
using System;
using System.Reflection;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class WorldStructuralContext : IStructuralContext
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public StructuralAdvice GetAdvice(Type type)
        {
#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
#endif

            var attributes = type.GetCustomAttributes(true);

#if DEBUG
            _logger.Info($"attributes.Length = {attributes.Length}");
#endif

            foreach (var attribute in attributes)
            {
#if DEBUG
                _logger.Info($"attribute.GetType().FullName = {attribute.GetType().FullName}");
#endif
            }

            var serializeOnlyExplicitlySerializedMembersAttribute = type.GetCustomAttribute<SerializeOnlyExplicitlySerializedMembersAttribute>(true);

#if DEBUG
            _logger.Info($"serializeOnlyExplicitlySerializedMembersAttribute = {serializeOnlyExplicitlySerializedMembersAttribute}");
#endif

            var worldRootAttribute = type.GetCustomAttribute<WorldRootAttribute>(true);

#if DEBUG
            _logger.Info($"worldRootAttribute = {worldRootAttribute}");
#endif

            throw new NotImplementedException("7CA8D943-D5C3-433F-96DE-6AEE55FEA1EB");
        }
    }
}
