using SymOntoClay.ActiveObject.MethodResponses;
using System.Threading.Tasks;

namespace TestSandbox.MethodResponsing
{
    public class TstIntMethodResponse : IAsyncMethodResponse<int>
    {
        public TstIntMethodResponse(Task<int> task)
        {
            Task = task;
        }

        public Task<int> Task { get; private set; }
        public int Result => Task.Result;

        public void Wait()
        {
            Task.Wait();
        }
    }
}
