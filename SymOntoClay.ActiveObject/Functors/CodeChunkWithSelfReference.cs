using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors
{
    //[SocSerialization]
    public partial class CodeChunkWithSelfReference : ICodeChunk
    {
        public CodeChunkWithSelfReference()
        {
        }

        public CodeChunkWithSelfReference(string id, ICodeChunksContext codeChunksFactory, Action<ICodeChunk> action)
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

    public partial class CodeChunkWithSelfReference<T1, T2, T3> : ICodeChunk
    {
        public CodeChunkWithSelfReference()
        {
        }

        public CodeChunkWithSelfReference(string id, ICodeChunksContext codeChunksFactory, T1 arg1, T2 arg2, T3 arg3, Action<ICodeChunk, T1, T2, T3> action)
        {
            _id = id;
            _codeChunksFactory = codeChunksFactory;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _action = action;
        }

        private string _id;
        private bool _isFinished;
        private bool _actionIsFinished;
        private ICodeChunksContext _codeChunksFactory;
        private T1 _arg1;
        private T2 _arg2;
        private T3 _arg3;
        private Action<ICodeChunk, T1, T2, T3> _action;
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
                _action(this, _arg1, _arg2, _arg3);

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
