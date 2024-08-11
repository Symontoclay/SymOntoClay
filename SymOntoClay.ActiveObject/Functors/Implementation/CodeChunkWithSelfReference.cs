using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public partial class CodeChunkWithSelfReference: ICodeChunkWithSelfReference
    {
        public CodeChunkWithSelfReference(string id, ICodeChunksContext codeChunksContext, Action<ICodeChunkWithSelfReference> action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private ICodeChunksContext _codeChunksContext;
        private Action<ICodeChunkWithSelfReference> _action;

        private bool _isFinished;
        private bool _actionIsFinished;
        private List<IBaseCodeChunk> _chunks = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunk(chunkId, _codeChunksContext, action);
            _chunks.Add(chunk);
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action)
        {
            var chunk = new CodeChunkWithSelfReference(chunkId, _codeChunksContext, action);
            _chunks.Add(chunk);
        }


    }
}
