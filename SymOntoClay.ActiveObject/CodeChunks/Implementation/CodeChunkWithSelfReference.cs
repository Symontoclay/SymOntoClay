using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class CodeChunkWithSelfReference : BaseCodeChunkWithSelfReference, ICodeChunkWithSelfReference
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

    public partial class CodeChunkWithSelfReference<T> : BaseCodeChunkWithSelfReference, ICodeChunkWithSelfReference<T>
    {
        public CodeChunkWithSelfReference(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Action<ICodeChunkWithSelfReference<T>, T> action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _arg1 = arg1;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private T _arg1;

        private ICodeChunksContext<T> _codeChunksContext;
        private Action<ICodeChunkWithSelfReference<T>, T> _action;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T> action)
        {
            AddChildCodeChunk(new CodeChunk<T>(chunkId, _codeChunksContext, _arg1, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T>, T> action)
        {
            AddChildCodeChunk(new CodeChunkWithSelfReference<T>(chunkId, _codeChunksContext, _arg1, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this, _arg1);
        }
    }

    public partial class CodeChunkWithSelfReference<T1, T2> : BaseCodeChunkWithSelfReference, ICodeChunkWithSelfReference<T1, T2>
    {
        public CodeChunkWithSelfReference(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Action<ICodeChunkWithSelfReference<T1, T2>, T1, T2> action)
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

        private ICodeChunksContext<T1, T2> _codeChunksContext;
        private Action<ICodeChunkWithSelfReference<T1, T2>, T1, T2> _action;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2> action)
        {
            AddChildCodeChunk(new CodeChunk<T1, T2>(chunkId, _codeChunksContext, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2>, T1, T2> action)
        {
            AddChildCodeChunk(new CodeChunkWithSelfReference<T1, T2>(chunkId, _codeChunksContext, _arg1, _arg2, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this, _arg1, _arg2);
        }
    }

    public partial class CodeChunkWithSelfReference<T1, T2, T3> : BaseCodeChunkWithSelfReference, ICodeChunkWithSelfReference<T1, T2, T3>
    {
        public CodeChunkWithSelfReference(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action)
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

        private ICodeChunksContext<T1, T2, T3> _codeChunksContext;
        private Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> _action;

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action)
        {
            AddChildCodeChunk(new CodeChunk<T1, T2, T3>(chunkId, _codeChunksContext, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference<T1, T2, T3>, T1, T2, T3> action)
        {
            AddChildCodeChunk(new CodeChunkWithSelfReference<T1, T2, T3>(chunkId, _codeChunksContext, _arg1, _arg2, _arg3, action));
        }

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(this, _arg1, _arg2, _arg3);
        }
    }
}
