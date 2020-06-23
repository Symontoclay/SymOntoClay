using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.CoreHelper.Threads;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor: BaseLoggedComponent
    {
        protected BaseThreadExecutor(IEntityLogger logger, IActivePeriodicObject activeObject)
            :base(logger)
        {
            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;
        }

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;

        protected IActivePeriodicObject _activeObject { get; private set; }

        public void SetCodeFrame(CodeFrame codeFrame)
        {
            _codeFrames.Push(codeFrame);
            _currentCodeFrame = codeFrame;
        }

        private bool CommandLoop()
        {
            try
            {
                var currentPosition = _currentCodeFrame.CurrentPosition;

#if DEBUG
                Log($"currentPosition = {currentPosition}");
#endif

                var compiledFunctionBodyCommands = _currentCodeFrame.CompiledFunctionBody.Commands;

#if DEBUG
                Log($"compiledFunctionBodyCommands.Count = {compiledFunctionBodyCommands.Count}");
#endif

                if (currentPosition >= compiledFunctionBodyCommands.Count)
                {
                    return false;
                }

                var currentCommand = compiledFunctionBodyCommands[currentPosition];

#if DEBUG
                Log($"currentCommand = {currentCommand}");
                Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                switch (currentCommand.OperationCode)
                {
                    case OperationCode.Nop:
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.PushVal:
                        _currentCodeFrame.ValuesStack.Push(currentCommand.Value);
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.CallBinOp:
                        switch(currentCommand.KindOfOperator)
                        {
                            case KindOfOperator.LeftRightStream:
                                ProcessLeftRightStream();
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(currentCommand.KindOfOperator), currentCommand.KindOfOperator, null);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentCommand.OperationCode), currentCommand.OperationCode, null);
                }

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Error(e);

                throw;
#endif
            }
        }

        private void ProcessLeftRightStream(Value leftOperand, Value rightOperand)
        {
#if DEBUG

#endif

            throw new NotImplementedException();
        }
    }
}
