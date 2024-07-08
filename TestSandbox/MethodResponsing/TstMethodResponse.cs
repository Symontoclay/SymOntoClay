using SymOntoClay.Core.Internal.Serialization.Functors;
using System.Threading.Tasks;

namespace TestSandbox.MethodResponsing
{
    public class TstMethodResponse: IMethodResponse
    {
        public TstMethodResponse(Task task)
        {
            Task = task;
        }

        public Task Task { get; private set; }
    }
}
