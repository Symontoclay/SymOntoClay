using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors
{
    //[SocSerialization]
    public partial class CodeChunkWithSelfReference : ICodeChunk
    {
        public CodeChunkWithSelfReference(ICodeChunksContext codeChunksFactory, string id, Action<ICodeChunk> action)
        {
            _id = id;
            _codeChunksFactory = codeChunksFactory;
            _action = action;
        }

        private string _id;
        private bool _isFinished;
        private bool _actionIsFinished;
        private ICodeChunksContext _codeChunksFactory;
        private Action<ICodeChunk> _action;
        private List<ICodeChunk> _children = new List<ICodeChunk>();

        /// <inheritdoc/>
        public void AddChild(ICodeChunk child)
        {
            if (_children.Contains(child))
            {
                return;
            }

            _children.Add(child);
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
            
            foreach(var child in _children)
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
