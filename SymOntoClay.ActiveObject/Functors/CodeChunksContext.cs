using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunksContext
    {
        private readonly List<CodeChunk> _chunks = new List<CodeChunk>();

        public void CreateCodeChunk(Action<CodeChunk> action)
        {
            var chunk = new CodeChunk(this, action);
            _chunks.Add(chunk);
        }

        public void Run()
        {
            foreach (var chunk in _chunks)
            { 
                chunk.Run();
            }
        }
    }
}
