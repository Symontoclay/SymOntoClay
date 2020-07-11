using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor: BaseLoggedComponent
    {
        protected BaseThreadExecutor(IEngineContext context, IActivePeriodicObject activeObject)
            :base(context.Logger)
        {
            _context = context;

            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;

            _operatorsResolver = context.DataResolversFactory.GetOperatorsResolver();
        }

        private IEngineContext _context;

        private readonly OperatorsResolver _operatorsResolver;

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;

        protected IActivePeriodicObject _activeObject { get; private set; }

        public void SetCodeFrame(CodeFrame codeFrame)
        {
            _codeFrames.Push(codeFrame);
            _currentCodeFrame = codeFrame;
        }

        public void SetCodeFrames(List<CodeFrame> codeFramesList)
        {
            var reverseCodeFramesList = codeFramesList.ToList();
            reverseCodeFramesList.Reverse();

            foreach(var codeFrame in reverseCodeFramesList)
            {
                SetCodeFrame(codeFrame);
            }
        }

        public Value Start()
        {
            return _activeObject.Start();
        }

        private bool CommandLoop()
        {
            try
            {
                if(_currentCodeFrame == null)
                {
#if DEBUG
                    Log("_currentCodeFrame == null return false;");
#endif

                    return false;
                }

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
#if DEBUG
                    Log("currentPosition >= compiledFunctionBodyCommands.Count return false;");
#endif

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

                    case OperationCode.ClearStack:
                        _currentCodeFrame.ValuesStack.Clear();
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.PushVal:
                        _currentCodeFrame.ValuesStack.Push(currentCommand.Value);
                        _currentCodeFrame.CurrentPosition++;
                        break;

                    case OperationCode.CallBinOp:
                        {
                            var paramsList = TakePositionedParameters(2);

#if DEBUG
                            Log($"paramsList = {paramsList.WriteListToString()}");
                            Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var operatorInfo = _operatorsResolver.GetOperator(currentCommand.KindOfOperator, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultFluentOptions());

#if DEBUG
                            Log($"operatorInfo = {operatorInfo}");
#endif

                            CallExecutable(operatorInfo, paramsList);

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.Return:
                        {
                            _codeFrames.Pop();

                            if(_codeFrames.Count == 0)
                            {
                                _currentCodeFrame = null;
                            }
                            else
                            {
                                _currentCodeFrame = _codeFrames.Peek();
                            }                      
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

        private List<Value> TakePositionedParameters(int count)
        {
            var result = new List<Value>();

            var valueStack = _currentCodeFrame.ValuesStack;

            for (var i = 0; i < count; i++)
            {
                result.Add(valueStack.Pop());
            }

            result.Reverse();

            return result;
        }

        private void CallExecutable(IExecutable executable, IList<Value> paramsList)
        {
            if(executable.IsSystemDefined)
            {
                var result = executable.SystemHandler.Call(paramsList, _currentCodeFrame.LocalContext);

#if DEBUG
                Log($"result = {result}");
#endif

                _currentCodeFrame.ValuesStack.Push(result);

#if DEBUG
                Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                return;
            }

            throw new NotImplementedException();
        }
    }
}
