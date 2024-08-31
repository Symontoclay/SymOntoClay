﻿using SymOntoClay.ActiveObject.MethodResponses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.CodeChunks.Implementation
{
    public abstract partial class BaseSyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference<MethodResult>
    {
        protected BaseSyncCallCodeChunkWithoutResultForMethodWithResultAndSelfReference(IBaseCodeChunksContext codeChunksContext)
        {
            _codeChunksContext = codeChunksContext;
        }

        private IBaseCodeChunksContext _codeChunksContext;

        public void Finish()
        {
            _isFinished = true;
            _codeChunksContext.Finish();
        }

        protected abstract ISyncMethodResponse<MethodResult> OnRunPreHandler();
        protected abstract void OnRunPostHandler(MethodResult methodResult);

        private bool _isPreHandlerFinished;
        private ISyncMethodResponse<MethodResult> _syncMethodResponse;
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