using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class CodeChunkWithResult<TResult> : BaseCodeChunkWithResult<TResult>, ICodeChunkWithResult<TResult>
    {
        public CodeChunkWithResult(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Action action)
            : base(codeChunksContext)
        {
            _id = id;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private Action _action;

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action();
        }
    }

    public class CodeChunkWithResult<T1, T2, T3, TResult> : BaseCodeChunkWithResult<TResult>, ICodeChunkWithResult<T1, T2, T3, TResult>
    {
        public CodeChunkWithResult(string id, ICodeChunksContextWithResult<T1, T2, T3, TResult> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action)
            : base(codeChunksContext)
        {
            _id = id;
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

        private Action<T1, T2, T3> _action;

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(_arg1, _arg2, _arg3);
        }
    }
}
