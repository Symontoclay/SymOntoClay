using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IStandardFactsBuilder
    {
        string BuildSayFactString(string selfId, string factStr);
        string BuildSoundFactString(double power, double distance, float directionToPosition, string factStr);
    }
}
