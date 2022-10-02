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
    public class Vector3AndIEntityConverter : BasePlatformTypesConverter
    {
        /// <inheritdoc/>
        public override Type PlatformType => typeof(Vector3);

        /// <inheritdoc/>
        public override Type CoreType => typeof(IEntity);

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
            var targetObject = (IEntity)coreObject;

            targetObject.Resolve();

            return targetObject.Position;
        }
    }
}
