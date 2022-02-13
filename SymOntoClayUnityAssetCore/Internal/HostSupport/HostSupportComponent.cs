/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
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
            _invokerInMainThread = worldContext.InvokerInMainThread;
            _platformSupport = platformSupport;
        }

        private readonly IInvokerInMainThread _invokerInMainThread;
        private readonly IPlatformSupport _platformSupport;

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(RelativeCoordinate relativeCoordinate)
        {
#if DEBUG
            //Log($"relativeCoordinate = {relativeCoordinate}");
#endif

            var invocableInMainThreadObj = new InvocableInMainThreadObj<Vector3>(() => {
                return _platformSupport.ConvertFromRelativeToAbsolute(relativeCoordinate);
            }, _invokerInMainThread);

            return invocableInMainThreadObj.Run();
        }

        /// <inheritdoc/>
        public Vector3 GetCurrentAbsolutePosition()
        {
            var invocableInMainThreadObj = new InvocableInMainThreadObj<Vector3>(() => {
                return _platformSupport.GetCurrentAbsolutePosition();
            }, _invokerInMainThread);

            return invocableInMainThreadObj.Run();
        }

        /// <inheritdoc/>
        public float GetDirectionToPosition(Vector3 position)
        {
            var invocableInMainThreadObj = new InvocableInMainThreadObj<float>(() => {
                return _platformSupport.GetDirectionToPosition(position);
            }, _invokerInMainThread);

            return invocableInMainThreadObj.Run();
        }
    }
}
