//using SymOntoClay.Serialization;
//using System;
//using System.Collections.Generic;

//namespace SymOntoClay.ActiveObject.Functors
//{
//    [SocSerialization]
//    public partial class CodeChunksContext: ICodeChunksContext
//    {
//        public CodeChunksContext()
//        {
//        }

//        public CodeChunksContext(string id)
//        {
//            _id = id;
//        }

//        public CodeChunksContext(string id, ICodeChunk parentCodeChunk)
//        {
//            throw new NotImplementedException();
//            //_id = id;
//        }

//        private string _id;
//        private List<ICodeChunk> _chunks = new List<ICodeChunk>();

//        public void CreateCodeChunk(string chunkId, Action action)
//        {
//            var chunk = new CodeChunk(chunkId, this, action);
//            _chunks.Add(chunk);
//        }

//        public void CreateCodeChunk(string chunkId, ICodeChunk parent, Action action)
//        {
//            throw new NotImplementedException();
//            //var chunk = new CodeChunk(chunkId, this, action);
//            //parent.AddChild(chunk);
//        }

//        public void CreateCodeChunk(string chunkId, Action<ICodeChunk> action)
//        {
//            var chunk = new CodeChunkWithSelfReference(chunkId, this, action);
//            _chunks.Add(chunk);
//        }

//        public void Finish(object result)
//        {
//            throw new NotImplementedException(_id);
//        }

//        public void Finish()
//        {
//            _isFinished = true;
//        }

//        private bool _isFinished;

//        public void Run()
//        {
//            foreach (var chunk in _chunks)
//            {
//                chunk.Run();

//                if(_isFinished)
//                {
//                    return;
//                }
//            }
//        }
//    }

//    public partial class CodeChunksContext<T1, T2, T3> : ICodeChunksContext
//    {
//        public CodeChunksContext()
//        {
//        }

//        public CodeChunksContext(string id, T1 arg1, T2 arg2, T3 arg3)
//        {
//            _id = id;
//            _arg1 = arg1;
//            _arg2 = arg2;
//            _arg3 = arg3;
//        }

//        public CodeChunksContext(string id, ICodeChunk parentCodeChunk)
//        {
//            throw new NotImplementedException();
//            //_id = id;
//        }

//        private string _id;
//        private T1 _arg1;
//        private T2 _arg2;
//        private T3 _arg3;
//        private List<ICodeChunk> _chunks = new List<ICodeChunk>();

//        public void CreateCodeChunk(string chunkId, Action<T1, T2, T3> action)
//        {
//            var chunk = new CodeChunk<T1, T2, T3>(chunkId, this, _arg1, _arg2, _arg3, action);
//            _chunks.Add(chunk);
//        }

//        public void CreateCodeChunk(string chunkId, ICodeChunk parent, Action<T1, T2, T3> action)
//        {
//            throw new NotImplementedException();
//            //var chunk = new CodeChunk<T1, T2, T3>(chunkId, this, _arg1, _arg2, _arg3, action);
//            //parent.AddChild(chunk);
//        }

//        public void CreateCodeChunk(string chunkId, Action<ICodeChunk, T1, T2, T3> action)
//        {
//            var chunk = new CodeChunkWithSelfReference<T1, T2, T3>(chunkId, this, _arg1, _arg2, _arg3, action);
//            _chunks.Add(chunk);
//        }

//        public void Finish(object result)
//        {
//            throw new NotImplementedException(_id);
//        }

//        public void Finish()
//        {
//            _isFinished = true;
//        }

//        private bool _isFinished;

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
