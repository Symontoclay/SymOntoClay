using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public partial class CodeChunksContext: ICodeChunksContext
    {
        public CodeChunksContext(string id)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;

        private List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunk(chunkId, this, action);
            _chunks.Add(chunk);
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action)
        {
            var chunk = new CodeChunkWithSelfReference(chunkId, this, action);
            _chunks.Add(chunk);
        }

        /// <inheritdoc/>
        public void Finish()
        {
            _isFinished = true;
        }

        private bool _isFinished;

        /// <inheritdoc/>
        public void Run()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Run();

                if (_isFinished)
                {
                    return;
                }
            }
        }
    }
}
