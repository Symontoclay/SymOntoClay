using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public class CodeChunkWithResultAndSelfReference<TResult>: BaseCodeChunkWithResultAndSelfReference<TResult>, ICodeChunkWithResultAndSelfReference<TResult>
    {
        public CodeChunkWithResultAndSelfReference(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
            : base(codeChunksContext)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private ICodeChunksContextWithResult<TResult> _codeChunksContext;
        private Action<ICodeChunkWithResultAndSelfReference<TResult>> _action;

        /// <inheritdoc/>
        public override void CreateCodeChunk(string chunkId, Action action)
        {
            AddChildCodeChunk(new CodeChunkWithResult<TResult>(chunkId, _codeChunksContext, action));
        }

        /// <inheritdoc/>
        public override void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            AddChildCodeChunk(new CodeChunkWithResultAndSelfReference<TResult>(chunkId, _codeChunksContext, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this);
        }
    }
}
