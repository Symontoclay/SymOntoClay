using SymOntoClay.Common.Cancellation;
using System.Collections.Generic;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IWorldContextSerializedEventsHandler
    {
        object GameComponentsListLockObj { get; }
        List<IGameComponent> GameComponentsForLateInitializingList { get; }
        CancellationTokenSourceContext StartedCancellationLinkedContext { get; }
    }
}
