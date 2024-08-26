using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class CodeChunkWithResultAndSelfReference<TResult> : BaseCodeChunkWithResultAndSelfReference<TResult>, ICodeChunkWithResultAndSelfReference<TResult>
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
        public void CreateCodeChunk(string chunkId, Action action)
        {
            AddChildCodeChunk(new CodeChunkWithResult<TResult>(chunkId, _codeChunksContext, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            AddChildCodeChunk(new CodeChunkWithResultAndSelfReference<TResult>(chunkId, _codeChunksContext, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this);
        }
    }

    public class CodeChunkWithResultAndSelfReference<T, TResult> : BaseCodeChunkWithResultAndSelfReference<TResult>, ICodeChunkWithResultAndSelfReference<T, TResult>
    {
        public CodeChunkWithResultAndSelfReference(string id, ICodeChunksContextWithResult<T, TResult> codeChunksContext, T arg1, Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> action)
            : base(codeChunksContext)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _arg1 = arg1;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private T _arg1;

        private ICodeChunksContextWithResult<T, TResult> _codeChunksContext;
        private Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> _action;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T> action)
        {
            AddChildCodeChunk(new CodeChunkWithResult<T, TResult>(chunkId, _codeChunksContext, _arg1, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T, TResult>, T> action)
        {
            AddChildCodeChunk(new CodeChunkWithResultAndSelfReference<T, TResult>(chunkId, _codeChunksContext, _arg1, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this, _arg1);
        }
    }

    public class CodeChunkWithResultAndSelfReference<T1, T2, TResult> : BaseCodeChunkWithResultAndSelfReference<TResult>, ICodeChunkWithResultAndSelfReference<T1, T2, TResult>
    {
        public CodeChunkWithResultAndSelfReference(string id, ICodeChunksContextWithResult<T1, T2, TResult> codeChunksContext, T1 arg1, T2 arg2, Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> action)
            : base(codeChunksContext)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _arg1 = arg1;
            _arg2 = arg2;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private T1 _arg1;
        private T2 _arg2;

        private ICodeChunksContextWithResult<T1, T2, TResult> _codeChunksContext;
        private Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> _action;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2> action)
        {
            AddChildCodeChunk(new CodeChunkWithResult<T1, T2, TResult>(chunkId, _codeChunksContext, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, TResult>, T1, T2> action)
        {
            AddChildCodeChunk(new CodeChunkWithResultAndSelfReference<T1, T2, TResult>(chunkId, _codeChunksContext, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this, _arg1, _arg2);
        }
    }

    public class CodeChunkWithResultAndSelfReference<T1, T2, T3, TResult> : BaseCodeChunkWithResultAndSelfReference<TResult>, ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>
    {
        public CodeChunkWithResultAndSelfReference(string id, ICodeChunksContextWithResult<T1, T2, T3, TResult> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action)
            : base(codeChunksContext)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;

        private ICodeChunksContextWithResult<T1, T2, T3, TResult> _codeChunksContext;
        private Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> _action;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action)
        {
            AddChildCodeChunk(new CodeChunkWithResult<T1, T2, T3, TResult>(chunkId, _codeChunksContext, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>, T1, T2, T3> action)
        {
            AddChildCodeChunk(new CodeChunkWithResultAndSelfReference<T1, T2, T3, TResult>(chunkId, _codeChunksContext, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this, _arg1, _arg2, _arg3);
        }
    }
}
