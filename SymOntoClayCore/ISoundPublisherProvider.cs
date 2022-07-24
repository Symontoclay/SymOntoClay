using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ISoundPublisherProvider
    {
        void PushSoundFact(float power, string factStr);
        void PushSoundFact(float power, RuleInstance fact);
        void PushSpeechFact(float power, string factStr);
        void PushSpeechFact(float power, RuleInstance fact);
    }
}
