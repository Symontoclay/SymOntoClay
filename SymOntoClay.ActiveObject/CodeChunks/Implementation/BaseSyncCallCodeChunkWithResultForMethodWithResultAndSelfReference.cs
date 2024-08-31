﻿using SymOntoClay.ActiveObject.MethodResponses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseSyncCallCodeChunkWithResultForMethodWithResultAndSelfReference<TResult, MethodResult>
    {
        public BaseSyncCallCodeChunkWithResultForMethodWithResultAndSelfReference(IBaseCodeChunksContextWithResult<TResult> codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        protected abstract ISyncMethodResponse<MethodResult> OnRunPreHandler();
        protected abstract void OnRunPostHandler(MethodResult methodResult);

        private bool _isPreHandlerFinished;
        private ISyncMethodResponse<MethodResult> _syncMethodResponse;
        private bool _isSyncMethodFinished;
        private bool _isPostHandlerFinished;
        private bool _isFinished;

        private IBaseCodeChunksContextWithResult<TResult> _codeChunksContext;

        private TResult _result = default;

        public TResult Result => _result;

        public void Finish(TResult result)
        {
            _result = result;
            _isFinished = true;
            _codeChunksContext.Finish(result);
        }

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
                _syncMethodResponse.Run();

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