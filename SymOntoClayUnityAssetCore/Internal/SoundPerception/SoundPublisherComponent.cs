using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.SoundPerception
{
    public class SoundPublisherComponent : BaseComponent
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

        public void PushSoundFact(float power, string text)
        {
#if DEBUG
            //Log($"power = {power}");
            //Log($"text = {text}");
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
