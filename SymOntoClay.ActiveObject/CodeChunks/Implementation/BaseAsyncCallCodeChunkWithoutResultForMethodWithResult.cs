﻿using SymOntoClay.ActiveObject.MethodResponses;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseAsyncCallCodeChunkWithoutResultForMethodWithResult<MethodResult>
    {
        protected abstract IMethodResponse<MethodResult> OnRunPreHandler();
        protected abstract void OnRunPostHandler(MethodResult methodResult);

        private bool _isPreHandlerFinished;
        private IMethodResponse<MethodResult> _syncMethodResponse;
        private bool _isSyncMethodFinished;
        private bool _isPostHandlerFinished;
        private bool _isFinished;

        public void Run()
        {
            if (_isFinished)
            {
                return;
            }

            if (!_isPreHandlerFinished)
            {
                _syncMethodResponse = OnRunPreHandler();

                _isPreHandlerFinished = true;
            }

            if (!_isSyncMethodFinished)
            {
                _syncMethodResponse.Wait();

                _isSyncMethodFinished = true;
            }

            if (!_isPostHandlerFinished)
            {
                OnRunPostHandler(_syncMethodResponse.Result);

                _isPostHandlerFinished = true;
            }

            _isFinished = true;
        }

        public bool IsFinished => _isFinished;
    }
}