using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public class CodeChunkWithResult<TResult> : ICodeChunkWithResult<TResult>
    {
        public CodeChunkWithResult(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Action action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private bool _isFinished;
        private ICodeChunksContextWithResult<TResult> _codeChunksContext;
        private Action _action;

        /// <inheritdoc/>
        public void Finish(TResult result)
        {
            _isFinished = true;
            _codeChunksContext.Finish(result);
        }

        /// <inheritdoc/>
        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            _action();

            _isFinished = true;
        }

        /// <inheritdoc/>
        public bool IsFinished => _isFinished;
    }
}
