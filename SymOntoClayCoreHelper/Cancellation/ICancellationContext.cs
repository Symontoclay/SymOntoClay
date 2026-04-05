using SymOntoClay.Common;
using System.Threading;

namespace SymOntoClay.CoreHelper.Cancellation
{
    public interface ICancellationContext: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString
    {
        bool IsCancellationRequested { get; }
        CancellationToken Token { get; }
    }
}
