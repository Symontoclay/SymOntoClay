//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SymOntoClay.ActiveObject.Functors
//{
//    public class CodeChunksContextWithResult<TResult> : ICodeChunksContext
//    {
//        public CodeChunksContextWithResult()
//        {
//        }

//        public CodeChunksContextWithResult(string id)
//        {
//            _id = id;
//        }

//        public CodeChunksContextWithResult(string id, ICodeChunk parentCodeChunk)
//        {
//            _id = id;
//        }

//        private string _id;
//        private readonly List<ICodeChunk> _chunks = new List<ICodeChunk>();

//        public void CreateCodeChunk(string chunkId, Action action)
//        {
//            var chunk = new CodeChunk(chunkId, this, action);
//            _chunks.Add(chunk);
//        }

//        public void CreateCodeChunk(string chunkId, Action<ICodeChunk> action)
//        {
//            var chunk = new CodeChunkWithSelfReference(chunkId, this, action);
//            _chunks.Add(chunk);
//        }

//        public void Finish()
//        {
//            _isFinished = true;
//        }

//        public void Finish(object result)
//        {
//            _result = (TResult)result;
//            _isFinished = true;
//        }

//        private bool _isFinished;
//        private TResult _result = default(TResult);

//        public TResult Result => _result;

//        public void Run()
//        {
//            foreach (var chunk in _chunks)
//            {
//                chunk.Run();

//                if (_isFinished)
//                {
//                    return;
//                }
//            }
//        }
//    }
//}
