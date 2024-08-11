using System;
using static System.Collections.Specialized.BitVector32;
using System.Security.Cryptography;
using SymOntoClay.Serialization;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public class CodeChunkWithResultAndSelfReference<TResult>: ICodeChunkWithResultAndSelfReference<TResult>
    {
        public CodeChunkWithResultAndSelfReference(string id, ICodeChunksContextWithResult<TResult> codeChunksContext, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            _id = id;
            _codeChunksContext = codeChunksContext;
            _action = action;
        }

        [SocSerializableActionKey]
        private string _id;

        private bool _isFinished;
        private bool _actionIsFinished;
        private ICodeChunksContextWithResult<TResult> _codeChunksContext;
        private Action<ICodeChunkWithResultAndSelfReference<TResult>> _action;

        private List<IBaseCodeChunk> _children = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunkWithResult<TResult>(chunkId, _codeChunksContext, action);
            _children.Add(chunk);
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithResultAndSelfReference<TResult>> action)
        {
            var chunk = new CodeChunkWithResultAndSelfReference<TResult>(chunkId, _codeChunksContext, action);
            _children.Add(chunk);
        }

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

            if (!_actionIsFinished)
            {
                _action(this);

                _actionIsFinished = true;
            }

            foreach (var child in _children)
            {
                if (child.IsFinished)
                {
                    continue;
                }

                child.Run();
            }

            _isFinished = true;
        }

        /// <inheritdoc/>
        public bool IsFinished => _isFinished;
    }
}
