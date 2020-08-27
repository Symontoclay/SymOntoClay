using SymOntoClay.Core;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TestSandbox.Helpers
{
    public class TstHostSupportComponent: IHostSupport
    {
        public TstHostSupportComponent(IPlatformSupport platformSupport)
        {
            _platformSupport = platformSupport;
        }

        private readonly IPlatformSupport _platformSupport;

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(Vector2 relativeCoordinates)
        {
            return _platformSupport.ConvertFromRelativeToAbsolute(relativeCoordinates);
        }
    }
}
