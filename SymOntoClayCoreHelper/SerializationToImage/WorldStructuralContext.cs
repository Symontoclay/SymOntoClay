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
        public KindOfStructuralContext Kind => KindOfStructuralContext.World;

        /// <inheritdoc/>
        public StructuralAdvice GetAdvice(Type type)
        {
#if DEBUG
            //_logger.Info($"type.FullName = {type.FullName}");
#endif

            var kindOfStructuralObject = GetKindOfStructuralObject(type);

#if DEBUG
            //_logger.Info($"kindOfStructuralObject = {kindOfStructuralObject}");
#endif

            switch(kindOfStructuralObject)
            {
                case KindOfStructuralObject.WorldRoot:
                    return new StructuralAdvice(KindOfSerializationStrategy.SerializeOnlyExplicitlySerializableMembers, kindOfStructuralObject);
            }

            /*var serializeOnlyExplicitlySerializedMembersAttribute = type.GetCustomAttribute<SerializeOnlyExplicitlySerializedMembersAttribute>(true);

#if DEBUG
            _logger.Info($"serializeOnlyExplicitlySerializedMembersAttribute = {serializeOnlyExplicitlySerializedMembersAttribute}");
#endif

            var worldRootAttribute = type.GetCustomAttribute<>(true);

#if DEBUG
            _logger.Info($"worldRootAttribute = {worldRootAttribute}");
#endif*/

            throw new NotImplementedException("7CA8D943-D5C3-433F-96DE-6AEE55FEA1EB");
        }

        /// <inheritdoc/>
        public KindOfStructuralObject GetKindOfStructuralObject(Type type)
        {
#if DEBUG
            //_logger.Info($"type.FullName = {type.FullName}");
#endif

            if(type.IsDefined(typeof(WorldRootAttribute), true))
            {
                return KindOfStructuralObject.WorldRoot;
            }

            if (type.IsDefined(typeof(WorldSettingsAttribute), true))
            {
                return KindOfStructuralObject.WorldSettings;
            }

            if (type.IsDefined(typeof(WorldComponentAttribute), true))
            {
                return KindOfStructuralObject.WorldComponent;
            }

            if (type.IsDefined(typeof(SerializeWithDataCreationAttribute), true))
            {
                return KindOfStructuralObject.SerializeWithSerializationDataCreation;
            }
            
            return KindOfStructuralObject.UsualObject;
        }
    }
}
