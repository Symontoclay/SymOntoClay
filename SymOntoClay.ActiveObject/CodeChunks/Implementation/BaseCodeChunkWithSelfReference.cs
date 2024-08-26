using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseCodeChunkWithSelfReference
    {
        private bool _isFinished;
        private bool _actionIsFinished;
        private List<IBaseCodeChunk> _children = new List<IBaseCodeChunk>();

        protected void AddChildCodeChunk(IBaseCodeChunk chunk)
        {
            _children.Add(chunk);
        }

        public void Finish()
        {
            _isFinished = true;
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

        public bool IsFinished => _isFinished;
    }
}
