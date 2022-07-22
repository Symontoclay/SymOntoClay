using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ISoundPublisherProvider
    {
        void PushSoundFact(float power, string text);
    }
}
