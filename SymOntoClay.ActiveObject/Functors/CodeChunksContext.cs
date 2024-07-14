using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Functors
{
    public class CodeChunksContext
    {
        public CodeChunksContext(string id)
        {

        }

        private readonly List<ICodeChunk> _chunks = new List<ICodeChunk>();

        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunk(this, chunkId, action);
            _chunks.Add(chunk);
        }

        public void CreateCodeChunk(string chunkId, Action<ICodeChunk> action)
        {
            var chunk = new CodeChunkWithSelfReference(this, chunkId, action);
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
