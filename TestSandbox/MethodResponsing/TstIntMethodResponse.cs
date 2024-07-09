using SymOntoClay.Core.Internal.Serialization.Functors;
using System.Threading.Tasks;

namespace TestSandbox.MethodResponsing
{
    public class TstIntMethodResponse: IMethodResponse<int>
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
