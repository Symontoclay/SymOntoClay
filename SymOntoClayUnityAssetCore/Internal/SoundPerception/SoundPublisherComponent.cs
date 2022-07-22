/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SoundPerception
{
    public class SoundPublisherComponent : BaseComponent, ISoundPublisherProvider
    {
        public SoundPublisherComponent(IEntityLogger logger, int instanceId, IHostSupport hostSupport, IWorldCoreGameComponentContext worldContext)
            : base(logger)
        {
            _instanceId = instanceId;
            _soundBus = worldContext.SoundBus;
            _hostSupport = hostSupport;
        }

        private readonly ISoundBus _soundBus;
        private readonly IHostSupport _hostSupport;
        private readonly int _instanceId;

        /// <inheritdoc/>
        public void PushSoundFact(float power, string text)
        {
#if DEBUG
            Log($"power = {power}");
            Log($"text = {text}");
#endif

            if(_soundBus == null)
            {
                return;
            }

#if DEBUG
            //Log("NEXT");
            //Log($"_hostSupport.GetCurrentAbsolutePosition() = {_hostSupport.GetCurrentAbsolutePosition()}");
#endif

            _soundBus.PushSound(_instanceId, power, _hostSupport.GetCurrentAbsolutePosition(), text);
        }
    }
}
