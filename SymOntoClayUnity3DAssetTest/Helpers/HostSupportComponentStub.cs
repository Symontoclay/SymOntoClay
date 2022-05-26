using SymOntoClay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests.Helpers
{
    public class HostSupportComponentStub : IHostSupport
    {
        public HostSupportComponentStub(IPlatformSupport platformSupport)
        {
            _platformSupport = platformSupport;
        }

        private readonly IPlatformSupport _platformSupport;

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(RelativeCoordinate relativeCoordinate)
        {
            return _platformSupport.ConvertFromRelativeToAbsolute(relativeCoordinate);
        }

        /// <inheritdoc/>
        public Vector3 GetCurrentAbsolutePosition()
        {
            return _platformSupport.GetCurrentAbsolutePosition();
        }

        /// <inheritdoc/>
        public float GetDirectionToPosition(Vector3 position)
        {
            return _platformSupport.GetDirectionToPosition(position);
        }
    }
}
