﻿using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public class CodeChunksContextWithResult<TResult> : BaseCodeChunksContextWithResult<TResult>, ICodeChunksContextWithResult<TResult>
    {
        public CodeChunksContextWithResult(string id)
        {
            _id = id;
        }

        [SocSerializableActionKey]
        private string _id;

        /// <inheritdoc/>
        public override void CreateCodeChunk(string chunkId, Action action)
        {
            AddCodeChunk(new CodeChunkWithResult<TResult>(chunkId, this, action));
        }

        /// <inheritdoc/>
        public override void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            AddCodeChunk(new CodeChunkWithResultAndSelfReference<TResult>(chunkId, this, action));
        }
    }
}
