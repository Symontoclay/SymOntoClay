﻿using SymOntoClay.Serialization;
using System;
using System.Collections.Generic;

namespace SymOntoClay.ActiveObject.Functors
{
    [SocSerialization]
    public partial class CodeChunksContext: ICodeChunksContext
    {
        public CodeChunksContext()
        {
        }

        public CodeChunksContext(string id)
        {
            _id = id;
        }

        public CodeChunksContext(string id, ICodeChunk parentCodeChunk)
        {
            _id = id;
        }

        private string _id;
        private List<ICodeChunk> _chunks = new List<ICodeChunk>();

        public void CreateCodeChunk(string chunkId, Action action)
        {
            var chunk = new CodeChunk(this, chunkId, action);
            _chunks.Add(chunk);
        }

        public void CreateCodeChunk(string chunkId, ICodeChunk parent, Action action)
        {
            var chunk = new CodeChunk(this, chunkId, action);
            parent.AddChild(chunk);
        }

        public void CreateCodeChunk(string chunkId, Action<ICodeChunk> action)
        {
            var chunk = new CodeChunkWithSelfReference(this, chunkId, action);
            _chunks.Add(chunk);
        }

        public void Finish(object result)
        {
            throw new NotImplementedException(_id);
        }

        public void Finish()
        {
            _isFinished = true;
        }

        private bool _isFinished;

        public void Run()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Run();

                if(_isFinished)
                {
                    return;
                }
            }
        }
    }
}
