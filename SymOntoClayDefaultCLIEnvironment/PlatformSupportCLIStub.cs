/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SymOntoClay.DefaultCLIEnvironment
{
    public class PlatformSupportCLIStub : IPlatformSupport
    {
        public PlatformSupportCLIStub()
            : this(new Vector3(10, 10, 10))
        {
        }

        public PlatformSupportCLIStub(Vector3 currentAbsolutePosition)
        {
            _currentAbsolutePosition = currentAbsolutePosition;
        }

        private readonly Vector3 _currentAbsolutePosition;

        /// <inheritdoc/>
        public Vector3 ConvertFromRelativeToAbsolute(IMonitorLogger logger, RelativeCoordinate relativeCoordinate)
        {
            return new Vector3(666, 999, 0);
        }

        /// <inheritdoc/>
        public Vector3 GetCurrentAbsolutePosition(IMonitorLogger logger)
        {
            return _currentAbsolutePosition;
        }

        /// <inheritdoc/>
        public float GetDirectionToPosition(IMonitorLogger logger, Vector3 position)
        {
            return 12;
        }

        /// <inheritdoc/>
        public bool CanBeTakenBy(IMonitorLogger logger, IEntity subject)
        {
            return true;
        }
    }
}
