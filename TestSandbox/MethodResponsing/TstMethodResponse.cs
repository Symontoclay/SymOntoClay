using SymOntoClay.ActiveObject.MethodResponses;
using System.Threading.Tasks;

namespace TestSandbox.MethodResponsing
{
    public class TstMethodResponse : IAsyncMethodResponse
    {
        public TstMethodResponse(Task task)
        {
            Task = task;
        }

        public Task Task { get; private set; }

        public void Wait()
        {
            Task.Wait();
        }
    }
}
