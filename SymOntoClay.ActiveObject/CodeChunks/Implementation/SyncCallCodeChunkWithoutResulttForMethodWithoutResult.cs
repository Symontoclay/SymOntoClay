using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Serialization;
using System;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public partial class SyncCallCodeChunkWithoutResulttForMethodWithoutResult: BaseSyncCallCodeChunkWithoutResulttForMethodWithoutResult, ISyncCallCodeChunkWithoutResultForMethodWithoutResult
    {
        public SyncCallCodeChunkWithoutResulttForMethodWithoutResult()
        {
        }

        public void Run()
        {
            throw new NotImplementedException("----CreateSyncCall|||||");
        }

        public bool IsFinished => throw new NotImplementedException("----CreateSyncCall|||||");
    }

    public partial class SyncCallCodeChunkWithoutResulttForMethodWithoutResult<T> : BaseSyncCallCodeChunkWithoutResulttForMethodWithoutResult, ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T>
    {
        public SyncCallCodeChunkWithoutResulttForMethodWithoutResult(string id, ICodeChunksContext<T> codeChunksContext, T arg1, Func<T, ISyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T _arg1;

        private Func<T, ISyncMethodResponse> _handler;

        private bool _isHandlerFinished;
        private ISyncMethodResponse _syncMethodResponse;

        public void Run()
        {
            if(!_isHandlerFinished)
            {
                _syncMethodResponse = _handler(_arg1);

                _isHandlerFinished = true;
            }

            throw new NotImplementedException("----CreateSyncCall|||||");
        }

        public bool IsFinished => throw new NotImplementedException("----CreateSyncCall|||||");
    }

    public partial class SyncCallCodeChunkWithoutResulttForMethodWithoutResult<T1, T2> : BaseSyncCallCodeChunkWithoutResulttForMethodWithoutResult, ISyncCallCodeChunkWithoutResultForMethodWithoutResult<T1, T2>
    {
        public SyncCallCodeChunkWithoutResulttForMethodWithoutResult(string id, ICodeChunksContext<T1, T2> codeChunksContext, T1 arg1, T2 arg2, Func<T1, T2, ISyncMethodResponse> handler)
        {
            _id = id;
            _arg1 = arg1;
            _arg2 = arg2;
            _handler = handler;
        }

        [SocSerializableActionKey]
        private string _id;

        private T1 _arg1;
        private T2 _arg2;

        private Func<T1, T2, ISyncMethodResponse> _handler;

        private bool _isHandlerFinished;
        private ISyncMethodResponse _syncMethodResponse;
        private bool _isSyncMethodFinished;
        private bool _isFinished;

        public void Run()
        {
            if(_isFinished)
            {
                return;
            }

            if (!_isHandlerFinished)
            {
                _syncMethodResponse = _handler(_arg1, _arg2);

                _isHandlerFinished = true;
            }

            if(!_isSyncMethodFinished)
            {
                _syncMethodResponse.Run();

                _isSyncMethodFinished = true;
            }

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}
