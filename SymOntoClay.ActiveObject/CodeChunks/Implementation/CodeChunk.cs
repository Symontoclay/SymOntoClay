using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class CodeChunk : BaseCodeChunk, ICodeChunk
    {
        public CodeChunk(string id, ICodeChunksContext codeChunksContext, Action action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private ICodeChunksContext _codeChunksContext;
        private Action _action;

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action();
        }
    }

    public partial class CodeChunk<T1, T2, T3> : BaseCodeChunk, ICodeChunk<T1, T2, T3>
    {
        public CodeChunk(string id, ICodeChunksContext<T1, T2, T3> codeChunksContext, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action)
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

        private ICodeChunksContext<T1, T2, T3> _codeChunksContext;
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
