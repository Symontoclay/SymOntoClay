using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor: BaseLoggedComponent
    {
        protected BaseThreadExecutor(IEngineContext context, IActivePeriodicObject activeObject)
            :base(context.Logger)
        {
            _context = context;

            _globalStorage = context.Storage.GlobalStorage;

            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;

            var dataResolversFactory = context.DataResolversFactory;

            _operatorsResolver = dataResolversFactory.GetOperatorsResolver();
            _logicalValueLinearResolver = dataResolversFactory.GetLogicalValueLinearResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        private IEngineContext _context;
        private IStorage _globalStorage;

        private readonly OperatorsResolver _operatorsResolver;
        private readonly LogicalValueLinearResolver _logicalValueLinearResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly VarsResolver _varsResolver;

        private Stack<CodeFrame> _codeFrames = new Stack<CodeFrame>();
        private CodeFrame _currentCodeFrame;

        protected IActivePeriodicObject _activeObject { get; private set; }

        public void SetCodeFrame(CodeFrame codeFrame, bool setAsRunning = true)
        {
#if DEBUG
            Log($"codeFrame = {codeFrame}");
#endif

            _codeFrames.Push(codeFrame);
            _currentCodeFrame = codeFrame;

            if(setAsRunning)
            {
                _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Running;
            }
        }

        public void SetCodeFrames(List<CodeFrame> codeFramesList)
        {
            var reverseCodeFramesList = codeFramesList.ToList();
            reverseCodeFramesList.Reverse();

            foreach(var codeFrame in reverseCodeFramesList)
            {
                codeFrame.ProcessInfo.Status = ProcessStatus.WaitingToRun;

                SetCodeFrame(codeFrame, false);
            }

            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Running;

#if DEBUG
            _context.InstancesStorage.PrintProcessesList();
#endif
        }

        public Value Start()
        {
            return _activeObject.Start();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _activeObject.Dispose();
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

                    case OperationCode.PushValFromVar:
                        {
                            var varName = currentCommand.Value.AsStrongIdentifierValue;

#if DEBUG
                            //Log($"varName = {varName}");
#endif

                            var targetValue = _varsResolver.GetVarValue(varName, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
                            //Log($"targetValue = {targetValue}");
#endif

                            _currentCodeFrame.ValuesStack.Push(targetValue);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.CallBinOp:
                        {
                            var paramsList = TakePositionedParameters(3);

#if DEBUG
                            //Log($"paramsList = {paramsList.WriteListToString()}");
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            var kindOfOperator = currentCommand.KindOfOperator;

                            if(kindOfOperator == KindOfOperator.IsNot)
                            {
                                kindOfOperator = KindOfOperator.Is;
                            }

                            var operatorInfo = _operatorsResolver.GetOperator(kindOfOperator, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
                            //Log($"operatorInfo = {operatorInfo}");
#endif

                            CallExecutable(operatorInfo, paramsList);

#if DEBUG
                            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            if(currentCommand.KindOfOperator == KindOfOperator.IsNot)
                            {
                                var result = _currentCodeFrame.ValuesStack.Pop();

#if DEBUG
                                //Log($"result = {result}");
#endif

                                result = result.AsLogicalValue.Inverse(_context);

#if DEBUG
                                //Log($"result (2) = {result}");
#endif

                                _currentCodeFrame.ValuesStack.Push(result);

#if DEBUG
                                //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif
                            }

                            _currentCodeFrame.CurrentPosition++;
                        }
                        break;

                    case OperationCode.UseInheritance:
                        ProcessUseInheritance();
                        break;

                    case OperationCode.UseNotInheritance:
                        ProcessUseNotInheritance();
                        break;

                    case OperationCode.AllocateAnonymousWaypoint:
                        {
                            if(currentCommand.CountMainParams == 0)
                            {
                                throw new NotImplementedException();
                                //break;
                            }

                            var paramsList = TakePositionedParameters(currentCommand.CountMainParams);

#if DEBUG
                            Log($"paramsList = {paramsList.WriteListToString()}");
                            Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                            switch(currentCommand.CountMainParams)
                            {
                                case 3:
                                    {
                                        var firstParam = paramsList[0];

                                        var resolvedFirstParam = _numberValueLinearResolver.Resolve(firstParam, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

                                        var secondParam = paramsList[1];

                                        var resolvedSecondParam = _numberValueLinearResolver.Resolve(secondParam, _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

                                        var annotationValue = paramsList[2].AsAnnotationValue;

                                        var value = new WaypointValue(new Vector2((float)(double)resolvedFirstParam.GetSystemValue(), (float)(double)resolvedSecondParam.GetSystemValue()), _context).GetIndexedValue(_context);

                                        _currentCodeFrame.ValuesStack.Push(value);

                                        _currentCodeFrame.CurrentPosition++;
                                    }
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(currentCommand.CountMainParams), currentCommand.CountMainParams, null);
                            }
                        }
                        break;

                    case OperationCode.Return:
                        {
#if DEBUG
                            Log("Begin case OperationCode.Return");
#endif

                            _currentCodeFrame.ProcessInfo.Status = ProcessStatus.Completed;

                            _codeFrames.Pop();

                            if(_codeFrames.Count == 0)
                            {
                                _currentCodeFrame = null;
                            }
                            else
                            {
                                _currentCodeFrame = _codeFrames.Peek();
                            }

#if DEBUG
                            _context.InstancesStorage.PrintProcessesList();

                            Log("End case OperationCode.Return");
#endif
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

        private List<IndexedValue> TakePositionedParameters(int count)
        {
            var result = new List<IndexedValue>();

            var valueStack = _currentCodeFrame.ValuesStack;

            for (var i = 0; i < count; i++)
            {
                result.Add(valueStack.Pop());
            }

            result.Reverse();

            return result;
        }

        private void CallExecutable(IExecutable executable, IList<IndexedValue> paramsList)
        {
            if(executable.IsSystemDefined)
            {
                var result = executable.SystemHandler.Call(paramsList, _currentCodeFrame.LocalContext);

#if DEBUG
                //Log($"result = {result}");
#endif

                _currentCodeFrame.ValuesStack.Push(result);

#if DEBUG
                //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

                return;
            }

            throw new NotImplementedException();
        }

        private void ProcessUseInheritance()
        {
            var paramsList = TakePositionedParameters(4);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = paramsList[0].AsStrongIdentifierValue;

            var superName = paramsList[1].AsStrongIdentifierValue;

#if DEBUG
            //Log($"paramsList[2] = {paramsList[2]}");
#endif

            var rank = _logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
            //Log($"rank = {rank}");
#endif

            inheritenceItem.SubName = subName.OriginalStrongIdentifierValue;
            inheritenceItem.SuperName = superName.OriginalStrongIdentifierValue;
            inheritenceItem.Rank = rank.OriginalLogicalValue;

#if DEBUG
            //Log($"inheritenceItem = {inheritenceItem}");
#endif

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }

        private void ProcessUseNotInheritance()
        {
            var paramsList = TakePositionedParameters(4);

#if DEBUG
            //Log($"paramsList = {paramsList.WriteListToString()}");
            //Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var inheritenceItem = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(inheritenceItem, _currentCodeFrame.LocalContext.Storage.DefaultSettingsOfCodeEntity);

            var subName = paramsList[0].AsStrongIdentifierValue;

            var superName = paramsList[1].AsStrongIdentifierValue;

#if DEBUG
            //Log($"paramsList[2] = {paramsList[2]}");
#endif

            var rank = _logicalValueLinearResolver.Resolve(paramsList[2], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions()).Inverse(_context);

#if DEBUG
            //Log($"subName = {subName}");
            //Log($"superName = {superName}");
            //Log($"rank = {rank}");
#endif

            inheritenceItem.SubName = subName.OriginalStrongIdentifierValue;
            inheritenceItem.SuperName = superName.OriginalStrongIdentifierValue;
            inheritenceItem.Rank = rank.OriginalLogicalValue;

#if DEBUG
            //Log($"inheritenceItem = {inheritenceItem}");
#endif

            _globalStorage.InheritanceStorage.SetInheritance(inheritenceItem);

            _currentCodeFrame.CurrentPosition++;
        }
    }
}
