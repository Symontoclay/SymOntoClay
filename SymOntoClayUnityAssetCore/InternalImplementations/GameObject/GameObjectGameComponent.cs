/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject
{
    public class GameObjectGameComponent: BaseStoredGameComponent
    {
        public GameObjectGameComponent(GameObjectSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
            _allowPublicPosition = settings.AllowPublicPosition;
            _useStaticPosition = settings.UseStaticPosition;

            _platformSupport = settings.PlatformSupport;

            _hostEndpointsRegistry = new EndpointsRegistry(Logger);

            var platformEndpointsList = EndpointDescriber.GetEndpointsInfoList(settings.HostListener);

            _hostEndpointsRegistry.AddEndpointsRange(platformEndpointsList);
        }

        private readonly IPlatformSupport _platformSupport;

        private readonly EndpointsRegistry _hostEndpointsRegistry;

        private readonly bool _allowPublicPosition;
        private readonly Vector3? _useStaticPosition;

        public IEndpointsRegistry EndpointsRegistry => _hostEndpointsRegistry;

        /// <inheritdoc/>
        public override bool IsWaited => true;

        /// <inheritdoc/>
        public override bool CanBeTakenBy(IMonitorLogger logger, IEntity subject)
        {
            if(_platformSupport == null)
            {
                return false;
            }

            return _platformSupport.CanBeTakenBy(logger, subject);
        }

        /// <inheritdoc/>
        public override Vector3? GetPosition(IMonitorLogger logger)
        {
            if(_allowPublicPosition)
            {
                if (_platformSupport == null)
                {
                    return _useStaticPosition;
                }

                return _platformSupport.GetCurrentAbsolutePosition(logger);
            }

            return null;
        }
    }
}
