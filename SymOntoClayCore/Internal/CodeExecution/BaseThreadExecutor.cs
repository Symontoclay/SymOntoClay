using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public abstract class BaseThreadExecutor: BaseLoggedComponent
    {
        private enum KindOfFunctionParameters
        {
            NoParameters,
            NamedParameters,
            PositionedParameters
        }

        protected BaseThreadExecutor(IEngineContext context, IActivePeriodicObject activeObject)
            :base(context.Logger)
        {
            _context = context;

            _globalStorage = context.Storage.GlobalStorage;
            _hostListener = context.HostListener;

            _instancesStorage = _context.InstancesStorage;

            _activeObject = activeObject;
            activeObject.PeriodicMethod = CommandLoop;

            var dataResolversFactory = context.DataResolversFactory;

            _operatorsResolver = dataResolversFactory.GetOperatorsResolver();
            _logicalValueLinearResolver = dataResolversFactory.GetLogicalValueLinearResolver();
            _numberValueLinearResolver = dataResolversFactory.GetNumberValueLinearResolver();
            _strongIdentifierLinearResolver = dataResolversFactory.GetStrongIdentifierLinearResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        private readonly IEngineContext _context;
        private readonly IStorage _globalStorage;
        private readonly IHostListener _hostListener;
        private readonly IInstancesStorageComponent _instancesStorage;

        private readonly OperatorsResolver _operatorsResolver;
        private readonly LogicalValueLinearResolver _logicalValueLinearResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly StrongIdentifierLinearResolver _strongIdentifierLinearResolver;
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
            _instancesStorage.PrintProcessesList();
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

                    case OperationCode.Call_MN:
                        CallFunction(KindOfFunctionParameters.NamedParameters, currentCommand.CountMainParams, KindOfFunctionParameters.NoParameters, currentCommand.CountAdditionalParams);
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
                            _instancesStorage.PrintProcessesList();

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

        private Dictionary<IndexedStrongIdentifierValue, IndexedValue> TakeNamedParameters(int count)
        {
            var rawParamsList = TakePositionedParameters(count * 2);

            var result = new Dictionary<IndexedStrongIdentifierValue, IndexedValue>();

            var enumerator = rawParamsList.GetEnumerator();

            while(enumerator.MoveNext())
            {
                var name = enumerator.Current.AsStrongIdentifierValue;

                enumerator.MoveNext();

                var value = enumerator.Current;

                result[name] = value;
            }

            return result;
        }

        private void CallFunction(KindOfFunctionParameters kindOfMainParameters, int mainParametersCount, KindOfFunctionParameters kindOfAdditionalParameters, int additionalParametersCount)
        {
#if DEBUG
            Log($"kindOfMainParameters = {kindOfMainParameters}");
            Log($"mainParametersCount = {mainParametersCount}");
            Log($"kindOfAdditionalParameters = {kindOfAdditionalParameters}");
            Log($"additionalParametersCount = {additionalParametersCount}");
#endif

            var valueStack = _currentCodeFrame.ValuesStack;

            var annotation = valueStack.Pop();

#if DEBUG
            Log($"annotation = {annotation}");
            Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            var caller = valueStack.Pop();

#if DEBUG
            Log($"caller = {caller}");
            Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
#endif

            Dictionary<IndexedStrongIdentifierValue, IndexedValue> mainNamedParameters = null;
            List<IndexedValue> mainPositionedParameters = null;

            Dictionary<IndexedStrongIdentifierValue, IndexedValue> additionalNamedParameters = null;
            List<IndexedValue> additionalPositionedParameters = null;

            switch (kindOfMainParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    mainNamedParameters = TakeNamedParameters(mainParametersCount);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    mainPositionedParameters = TakePositionedParameters(mainParametersCount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfMainParameters), kindOfMainParameters, null);
            }

            switch (kindOfAdditionalParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    additionalNamedParameters = TakeNamedParameters(additionalParametersCount);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    additionalPositionedParameters = TakePositionedParameters(additionalParametersCount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfAdditionalParameters), kindOfAdditionalParameters, null);
            }

#if DEBUG
            Log($"_currentCodeFrame = {_currentCodeFrame.ToDbgString()}");
            Log($"mainNamedParameters = {mainNamedParameters.WriteDict_1_ToString()}");
            Log($"additionalNamedParameters = {additionalNamedParameters.WriteDict_1_ToString()}");
#endif

            if(caller.IsPointRefValue)
            {
                CallPointRefValue(caller.AsPointRefValue, kindOfMainParameters, mainNamedParameters, mainPositionedParameters, kindOfAdditionalParameters, additionalNamedParameters, additionalPositionedParameters);
                return;
            }

            throw new NotImplementedException();
        }

        private void CallPointRefValue(IndexedPointRefValue caller, 
            KindOfFunctionParameters kindOfMainParameters, Dictionary<IndexedStrongIdentifierValue, IndexedValue> mainNamedParameters, List<IndexedValue> mainPositionedParameters,
            KindOfFunctionParameters kindOfAdditionalParameters, Dictionary<IndexedStrongIdentifierValue, IndexedValue> additionalNamedParameters, List<IndexedValue> additionalPositionedParameters)
        {
#if DEBUG
            Log($"caller.LeftOperand = {caller.LeftOperand}");
#endif

            if(caller.LeftOperand.IsHostValue)
            {
                CallHost(caller.RightOperand.AsStrongIdentifierValue, kindOfMainParameters, mainNamedParameters, mainPositionedParameters, kindOfAdditionalParameters, additionalNamedParameters, additionalPositionedParameters);
            }

            throw new NotImplementedException();
        }

        private void CallHost(IndexedStrongIdentifierValue methodName, KindOfFunctionParameters kindOfMainParameters, Dictionary<IndexedStrongIdentifierValue, IndexedValue> mainNamedParameters, List<IndexedValue> mainPositionedParameters,
            KindOfFunctionParameters kindOfAdditionalParameters, Dictionary<IndexedStrongIdentifierValue, IndexedValue> additionalNamedParameters, List<IndexedValue> additionalPositionedParameters)
        {
#if DEBUG
            Log($"methodName = {methodName}");
            Log($"kindOfMainParameters = {kindOfMainParameters}");
            Log($"mainNamedParameters = {mainNamedParameters.WriteDict_1_ToString()}");
            Log($"mainPositionedParameters = {mainPositionedParameters.WriteListToString()}");
            Log($"kindOfAdditionalParameters = {kindOfAdditionalParameters}");
            Log($"additionalNamedParameters = {additionalNamedParameters.WriteDict_1_ToString()}");
            Log($"additionalPositionedParameters = {additionalPositionedParameters.WriteListToString()}");
#endif

            var command = new Command();
            command.Name = methodName.OriginalStrongIdentifierValue;

            switch(kindOfMainParameters)
            {
                case KindOfFunctionParameters.NoParameters:
                    break;

                case KindOfFunctionParameters.NamedParameters:
                    command.ParamsDict = mainNamedParameters.ToDictionary(p => p.Key.OriginalStrongIdentifierValue, p => p.Value.OriginalValue);
                    break;

                case KindOfFunctionParameters.PositionedParameters:
                    command.ParamsList = mainPositionedParameters.Select(p => p.OriginalValue).ToList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfMainParameters), kindOfMainParameters, null);
            }

#if DEBUG
            Log($"command = {command}");
#endif
            var processCreatingResult = _hostListener.CreateProcess(command);

#if DEBUG
            Log($"processCreatingResult = {processCreatingResult}");
#endif

            if(processCreatingResult.IsSuccessful)
            {
                var processInfo = processCreatingResult.Process;

                _instancesStorage.AppendAndTryStartProcessInfo(processInfo);
                Task.WaitAll();
            }            

            throw new NotImplementedException();
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

            var subName = _strongIdentifierLinearResolver.Resolve(paramsList[0], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

            var superName = _strongIdentifierLinearResolver.Resolve(paramsList[1], _currentCodeFrame.LocalContext, ResolverOptions.GetDefaultOptions());

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
