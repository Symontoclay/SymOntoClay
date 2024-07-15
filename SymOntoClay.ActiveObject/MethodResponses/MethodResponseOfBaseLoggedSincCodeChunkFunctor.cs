using SymOntoClay.ActiveObject.Functors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.ActiveObject.MethodResponses
{
    public class MethodResponseOfBaseLoggedSincCodeChunkFunctor : IMethodResponse
    {
        public MethodResponseOfBaseLoggedSincCodeChunkFunctor(BaseLoggedSincCodeChunkFunctor source)
        {
            _source = source;
        }

        private readonly BaseLoggedSincCodeChunkFunctor _source;

        /// <inheritdoc/>
        public Task Task => Task.CompletedTask;

        /// <inheritdoc/>
        public void Wait()
        {
        }
    }
}
