using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public class CodeChunk : BaseCodeChunk, ICodeChunk
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

    public class CodeChunk<T> : BaseCodeChunk, ICodeChunk<T>
    {
        public CodeChunk(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Action<T> action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _arg1 = arg1;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private ICodeChunksContext<T> _codeChunksContext;
        private T _arg1;
        private Action<T> _action;

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(_arg1);
        }
    }

    public class CodeChunk<T1, T2> : BaseCodeChunk, ICodeChunk<T1, T2>
    {
        public CodeChunk(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Action<T1, T2> action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _arg1 = arg1;
            _arg2 = arg2;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private ICodeChunksContext<T1, T2> _codeChunksContext;
        private T1 _arg1;
        private T2 _arg2;
        private Action<T1, T2> _action;

        /// <inheritdoc/>
        protected override void OnRunAction()
        {
            _action(_arg1, _arg2);
        }
    }

    public class CodeChunk<T1, T2, T3> : BaseCodeChunk, ICodeChunk<T1, T2, T3>
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
