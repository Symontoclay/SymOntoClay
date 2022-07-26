using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IStandardFactsBuilder
    {
        string BuildSayFactString(string selfId, string factStr);
        RuleInstance BuildSayFactInstance(string selfId, RuleInstance fact);
        string BuildSoundFactString(double power, double distance, float directionToPosition, string factStr);
        RuleInstance BuildSoundFactInstance(double power, double distance, float directionToPosition, RuleInstance fact);
    }
}
