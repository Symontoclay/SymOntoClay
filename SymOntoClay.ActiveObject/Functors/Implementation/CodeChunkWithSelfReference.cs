using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public partial class CodeChunkWithSelfReference: BaseCodeChunkWithSelfReference, ICodeChunkWithSelfReference
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

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            AddChildCodeChunk(new CodeChunk(chunkId, _codeChunksContext, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action)
        {
            AddChildCodeChunk(new CodeChunkWithSelfReference(chunkId, _codeChunksContext, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this);
        }
    }
}
