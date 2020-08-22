using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.HostSupport
{
    public class HostSupportComponent : BaseComponent, IHostSupport
    {
        public HostSupportComponent(IEntityLogger logger, IPlatformSupport platformSupport, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _worldContext = worldContext;
        }

        private readonly IWorldCoreGameComponentContext _worldContext;
        private readonly IPlatformSupport _platformSupport;

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates)
        {
            return _platformSupport.ConvertFromRelativeToAbsolute(relativeCoordinates);
        }
    }
}
