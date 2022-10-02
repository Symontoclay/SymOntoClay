using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters
{
    [PlatformTypesConverter]
    public class Vector3AndStrongIdentifierValueConverter: BasePlatformTypesConverter
    {
        /// <inheritdoc/>
        public override Type PlatformType => typeof(Vector3);

        /// <inheritdoc/>
        public override Type CoreType => typeof(StrongIdentifierValue);

        /// <inheritdoc/>
        public override bool CanConvertToPlatformType => true;

        /// <inheritdoc/>
        public override bool CanConvertToCoreType => false;

        /// <inheritdoc/>
        public override object ConvertToCoreType(object platformObject, IEntityLogger logger)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object ConvertToPlatformType(object coreObject, IEntityLogger logger)
        {
            var identifier = (StrongIdentifierValue)coreObject;

#if DEBUG
            logger.Log($"identifier = {identifier}");
#endif

            var kindOfName = identifier.KindOfName;

            switch(kindOfName)
            {
                case KindOfName.Entity:
                    return ConvertEntityToPlatformType(identifier, logger);

                case KindOfName.Concept:
                    return ConvertConceptToPlatformType(identifier, logger);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private object ConvertEntityToPlatformType(StrongIdentifierValue identifier, IEntityLogger logger)
        {
            throw new NotImplementedException();
        }

        private object ConvertConceptToPlatformType(StrongIdentifierValue identifier, IEntityLogger logger)
        {
            throw new NotImplementedException();
        }
    }
}
