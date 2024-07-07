using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
