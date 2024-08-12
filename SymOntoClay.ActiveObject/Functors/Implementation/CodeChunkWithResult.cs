using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;

namespace SymOntoClay.ActiveObject.Functors.Implementation
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
}
