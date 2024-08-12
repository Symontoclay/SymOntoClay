using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public partial class CodeChunksContext: BaseCodeChunksContext, ICodeChunksContext
    {
        public CodeChunksContext(string id)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;

        /// <inheritdoc/>
        public override void CreateCodeChunk(string chunkId, Action action)
        {
            AddCodeChunk(new CodeChunk(chunkId, this, action));
        }

        /// <inheritdoc/>
        public override void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action)
        {
            AddCodeChunk(new CodeChunkWithSelfReference(chunkId, this, action));
        }
    }
}
