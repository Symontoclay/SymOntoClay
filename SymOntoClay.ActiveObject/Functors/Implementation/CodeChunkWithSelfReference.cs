using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace SymOntoClay.ActiveObject.Functors.Implementation
{
    public partial class CodeChunkWithSelfReference: ICodeChunkWithSelfReference
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

        private bool _isFinished;
        private bool _actionIsFinished;
        private List<IBaseCodeChunk> _children = new List<IBaseCodeChunk>();

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunk(chunkId, _codeChunksContext, action);
            _children.Add(chunk);
        }

        /// <inheritdoc/>
        public void CreateCodeChunk(string chunkId, Action<ICodeChunkWithSelfReference> action)
        {
            var chunk = new CodeChunkWithSelfReference(chunkId, _codeChunksContext, action);
            _children.Add(chunk);
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
