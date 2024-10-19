using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract class BaseCodeChunkWithSelfReference
    {
        protected BaseCodeChunkWithSelfReference(IBaseCodeChunksContext codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        private bool _isFinished;
        private bool _actionIsFinished;

        private IBaseCodeChunksContext _codeChunksContext;

        private List<IBaseCodeChunk> _children = new List<IBaseCodeChunk>();

        protected void AddChildCodeChunk(IBaseCodeChunk chunk)
        {
            _children.Add(chunk);
        }

        public void Finish()
        {
            _isFinished = true;
            _codeChunksContext.Finish();
        }

        protected abstract void OnRunAction();

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            if (!_actionIsFinished)
            {
                OnRunAction();

                _actionIsFinished = true;
            }

            if (_isFinished)
            {
                return;
            }

            foreach (var child in _children)
            {
                if (_isFinished)
                {
                    return;
                }

                if (child.IsFinished)
                {
                    continue;
                }

                child.Run();
            }

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}
